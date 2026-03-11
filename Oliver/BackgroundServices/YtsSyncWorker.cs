using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Oliver.Data;
using Oliver.Domain;
using Oliver.Domain.YTS;
using Oliver.Services;

namespace Oliver.BackgroundServices;

public class YtsSyncWorker : BackgroundService {
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly ILogger<YtsSyncWorker> _logger;
	private readonly IConfiguration _config;

	public YtsSyncWorker(IServiceScopeFactory scopeFactory, ILogger<YtsSyncWorker> logger, IConfiguration config) {
		_scopeFactory = scopeFactory;
		_logger = logger;
		_config = config;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		_logger.LogInformation("YTS sync worker starting");

		// Wait a moment for the app to fully start
		await Task.Delay(2000, stoppingToken);

		try {
			await SyncAllMoviesAsync(stoppingToken);
			await DownloadAllTorrentsAsync(stoppingToken);
			await DownloadAllImagesAsync(stoppingToken);

			_logger.LogInformation("YTS sync complete! All data downloaded.");
		} catch (OperationCanceledException) {
			_logger.LogInformation("YTS sync worker cancelled");
		} catch (Exception ex) {
			_logger.LogError(ex, "YTS sync worker encountered a fatal error");
		}
	}

	private async Task SyncAllMoviesAsync(CancellationToken ct) {
		using var scope = _scopeFactory.CreateScope();
		var api = scope.ServiceProvider.GetRequiredService<YtsApiClient>();
		var db = scope.ServiceProvider.GetRequiredService<OliverContext>();

		var pageSize = _config.GetValue("Yts:PageSize", 50);
		var delayMs = _config.GetValue("Yts:RequestDelayMs", 1000);

		// Get the last synced page from state
		var startPage = await GetSyncStateAsync(db, "LastCompletedPage", 0);
		startPage++; // Start from the next page

		_logger.LogInformation("Starting movie sync from page {Page}", startPage);

		// First request to get total count
		var (firstMovies, totalCount) = await api.GetMoviesPageAsync(startPage, pageSize, ct);
		if (totalCount == 0) {
			_logger.LogWarning("API returned 0 movies. Check if the API is available.");
			return;
		}

		var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
		_logger.LogInformation("Total movies: {Count}, Total pages: {Pages}, Starting at: {Start}", totalCount, totalPages, startPage);

		// Process the first page
		var newMovies = await UpsertMoviesAsync(db, firstMovies);
		_logger.LogInformation("Page {Page}/{Total}: {New} new/updated movies", startPage, totalPages, newMovies);
		await SetSyncStateAsync(db, "LastCompletedPage", startPage);

		// Process remaining pages
		for (var page = startPage + 1; page <= totalPages; page++) {
			ct.ThrowIfCancellationRequested();
			await Task.Delay(delayMs, ct);

			var (movies, _) = await api.GetMoviesPageAsync(page, pageSize, ct);
			if (movies.Count == 0) {
				_logger.LogWarning("Empty page {Page}, stopping sync", page);
				break;
			}

			newMovies = await UpsertMoviesAsync(db, movies);
			_logger.LogInformation("Page {Page}/{Total}: {New} new/updated movies (total in DB: {DbCount})",
				page, totalPages, newMovies, await db.Movies.CountAsync(ct));

			await SetSyncStateAsync(db, "LastCompletedPage", page);
		}

		var movieCount = await db.Movies.CountAsync(ct);
		var torrentCount = await db.TorrentInfos.CountAsync(ct);
		_logger.LogInformation("Movie sync complete: {Movies} movies, {Torrents} torrents in database", movieCount, torrentCount);
	}

	private async Task DownloadAllTorrentsAsync(CancellationToken ct) {
		using var scope = _scopeFactory.CreateScope();
		var downloader = scope.ServiceProvider.GetRequiredService<DownloadService>();
		var db = scope.ServiceProvider.GetRequiredService<OliverContext>();

		var delayMs = _config.GetValue("Yts:RequestDelayMs", 1000);

		var torrentsToDownload = await db.TorrentInfos
			.Where(t => !t.TorrentFileDownloaded && t.Url != null)
			.ToListAsync(ct);

		_logger.LogInformation("Downloading {Count} torrent files", torrentsToDownload.Count);

		var downloaded = 0;
		var failed = 0;

		foreach (var torrent in torrentsToDownload) {
			ct.ThrowIfCancellationRequested();

			var path = await downloader.DownloadTorrentAsync(torrent.Hash, new Uri(torrent.Url!), ct);
			if (path is not null) {
				torrent.TorrentFileDownloaded = true;
				torrent.TorrentFilePath = path;
				downloaded++;
			} else {
				failed++;
			}

			if (downloaded % 100 == 0) {
				await db.SaveChangesAsync(ct);
				_logger.LogInformation("Torrents: {Downloaded} downloaded, {Failed} failed, {Remaining} remaining",
					downloaded, failed, torrentsToDownload.Count - downloaded - failed);
			}

			await Task.Delay(delayMs / 2, ct);
		}

		await db.SaveChangesAsync(ct);
		_logger.LogInformation("Torrent download complete: {Downloaded} downloaded, {Failed} failed", downloaded, failed);
	}

	private async Task DownloadAllImagesAsync(CancellationToken ct) {
		using var scope = _scopeFactory.CreateScope();
		var downloader = scope.ServiceProvider.GetRequiredService<DownloadService>();
		var db = scope.ServiceProvider.GetRequiredService<OliverContext>();

		var delayMs = _config.GetValue("Yts:RequestDelayMs", 1000);

		var moviesToDownload = await db.Movies
			.Where(m => !m.ImagesDownloaded)
			.ToListAsync(ct);

		_logger.LogInformation("Downloading images for {Count} movies", moviesToDownload.Count);

		var completed = 0;

		foreach (var movie in moviesToDownload) {
			ct.ThrowIfCancellationRequested();

			var imageUrls = new Dictionary<string, string?> {
				["small_cover"] = movie.SmallCoverImage,
				["medium_cover"] = movie.MediumCoverImage,
				["large_cover"] = movie.LargeCoverImage,
				["background"] = movie.BackgroundImage,
				["background_original"] = movie.BackgroundImageOriginal,
			};

			await downloader.DownloadMovieImagesAsync(movie.YtsId, imageUrls, ct);
			movie.ImagesDownloaded = true;
			completed++;

			if (completed % 100 == 0) {
				await db.SaveChangesAsync(ct);
				_logger.LogInformation("Images: {Completed}/{Total} movies processed", completed, moviesToDownload.Count);
			}

			await Task.Delay(delayMs / 2, ct);
		}

		await db.SaveChangesAsync(ct);
		_logger.LogInformation("Image download complete: {Count} movies processed", completed);
	}

	private static async Task<int> UpsertMoviesAsync(OliverContext db, List<YtsMovie> dtos) {
		var count = 0;

		foreach (var dto in dtos) {
			var existing = await db.Movies
				.Include(m => m.Torrents)
				.Include(m => m.Genres)
				.FirstOrDefaultAsync(m => m.YtsId == dto.Id);

			if (existing is null) {
				var movie = MapToMovie(dto);
				db.Movies.Add(movie);
				count++;
			} else {
				UpdateMovie(existing, dto);
				count++;
			}
		}

		await db.SaveChangesAsync();
		return count;
	}

	private static Movie MapToMovie(YtsMovie dto) {
		var movie = new Movie {
			YtsId = dto.Id,
			Url = dto.Url,
			ImdbCode = dto.ImdbCode,
			Title = dto.Title,
			TitleEnglish = dto.TitleEnglish,
			TitleLong = dto.TitleLong,
			Slug = dto.Slug,
			Year = dto.Year,
			Rating = dto.Rating,
			Runtime = dto.Runtime,
			Summary = dto.Summary,
			DescriptionFull = dto.DescriptionFull,
			Synopsis = dto.Synopsis,
			YtTrailerCode = dto.YtTrailerCode,
			Language = dto.Language,
			MpaRating = dto.MpaRating,
			BackgroundImage = dto.BackgroundImage,
			BackgroundImageOriginal = dto.BackgroundImageOriginal,
			SmallCoverImage = dto.SmallCoverImage,
			MediumCoverImage = dto.MediumCoverImage,
			LargeCoverImage = dto.LargeCoverImage,
			State = dto.State,
			DateUploaded = dto.DateUploaded,
			DateUploadedUnix = dto.DateUploadedUnix,
			LastSyncedAt = DateTime.UtcNow,
		};

		if (dto.Genres is not null) {
			movie.Genres = dto.Genres.Select(g => new Genre { Name = g }).ToList();
		}

		if (dto.Torrents is not null) {
			movie.Torrents = dto.Torrents.Select(t => new TorrentInfo {
				Url = t.Url,
				Hash = t.Hash,
				Quality = t.Quality,
				Type = t.Type,
				Size = t.Size,
				SizeBytes = t.SizeBytes,
				Seeds = t.Seeds,
				Peers = t.Peers,
				DateUploaded = t.DateUploaded,
				DateUploadedUnix = t.DateUploadedUnix,
			}).ToList();
		}

		return movie;
	}

	private static void UpdateMovie(Movie movie, YtsMovie dto) {
		movie.Url = dto.Url;
		movie.ImdbCode = dto.ImdbCode;
		movie.Title = dto.Title;
		movie.TitleEnglish = dto.TitleEnglish;
		movie.TitleLong = dto.TitleLong;
		movie.Slug = dto.Slug;
		movie.Year = dto.Year;
		movie.Rating = dto.Rating;
		movie.Runtime = dto.Runtime;
		movie.Summary = dto.Summary;
		movie.DescriptionFull = dto.DescriptionFull;
		movie.Synopsis = dto.Synopsis;
		movie.YtTrailerCode = dto.YtTrailerCode;
		movie.Language = dto.Language;
		movie.MpaRating = dto.MpaRating;
		movie.BackgroundImage = dto.BackgroundImage;
		movie.BackgroundImageOriginal = dto.BackgroundImageOriginal;
		movie.SmallCoverImage = dto.SmallCoverImage;
		movie.MediumCoverImage = dto.MediumCoverImage;
		movie.LargeCoverImage = dto.LargeCoverImage;
		movie.State = dto.State;
		movie.DateUploaded = dto.DateUploaded;
		movie.DateUploadedUnix = dto.DateUploadedUnix;
		movie.LastSyncedAt = DateTime.UtcNow;

		// Sync torrents — add new ones
		if (dto.Torrents is not null) {
			var existingHashes = movie.Torrents.Select(t => t.Hash).ToHashSet();
			foreach (var t in dto.Torrents) {
				if (!existingHashes.Contains(t.Hash)) {
					movie.Torrents.Add(new TorrentInfo {
						Url = t.Url,
						Hash = t.Hash,
						Quality = t.Quality,
						Type = t.Type,
						Size = t.Size,
						SizeBytes = t.SizeBytes,
						Seeds = t.Seeds,
						Peers = t.Peers,
						DateUploaded = t.DateUploaded,
						DateUploadedUnix = t.DateUploadedUnix,
					});
				}
			}
		}

		// Sync genres
		if (dto.Genres is not null) {
			var existingGenres = movie.Genres.Select(g => g.Name).ToHashSet();
			foreach (var g in dto.Genres) {
				if (!existingGenres.Contains(g)) {
					movie.Genres.Add(new Genre { Name = g });
				}
			}
		}
	}

	private static async Task<int> GetSyncStateAsync(OliverContext db, string key, int defaultValue) {
		var state = await db.SyncStates.FirstOrDefaultAsync(s => s.Key == key);
		if (state is null || !int.TryParse(state.Value, out var value)) {
			return defaultValue;
		}
		return value;
	}

	private static async Task SetSyncStateAsync(OliverContext db, string key, int value) {
		var state = await db.SyncStates.FirstOrDefaultAsync(s => s.Key == key);
		if (state is null) {
			db.SyncStates.Add(new SyncState { Key = key, Value = value.ToString(CultureInfo.InvariantCulture) });
		} else {
			state.Value = value.ToString(CultureInfo.InvariantCulture);
		}
		await db.SaveChangesAsync();
	}
}
