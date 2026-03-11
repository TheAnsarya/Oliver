using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Oliver.Data;

namespace Oliver.Services;

public class DataVerificationService(IServiceScopeFactory scopeFactory, IConfiguration configuration, TorrentParsingService torrentParser) {
	private static ReadOnlySpan<byte> JpegMagic => [0xff, 0xd8, 0xff];
	private static ReadOnlySpan<byte> PngMagic => [0x89, 0x50, 0x4e, 0x47];

	private readonly string _basePath = Path.GetFullPath(configuration["Downloads:BasePath"] ?? "YtsData");
	private readonly string _torrentsFolder = configuration["Downloads:TorrentsFolder"] ?? "torrents";
	private readonly string _imagesFolder = configuration["Downloads:ImagesFolder"] ?? "images";

	public async Task<MovieCountVerification> VerifyMovieCountAsync(CancellationToken ct = default) {
		using var scope = scopeFactory.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<OliverContext>();
		var apiClient = scope.ServiceProvider.GetRequiredService<YtsApiClient>();

		var dbCount = await db.Movies.CountAsync(ct);

		// Fetch page 1 to get total count from API
		var (_, apiTotal) = await apiClient.GetMoviesPageAsync(1, 1, ct);

		return new MovieCountVerification {
			DatabaseCount = dbCount,
			ApiTotalCount = apiTotal,
			Missing = Math.Max(0, apiTotal - dbCount),
			IsComplete = dbCount >= apiTotal,
		};
	}

	public async Task<TorrentValidationResult> ValidateTorrentFilesAsync(CancellationToken ct = default) {
		using var scope = scopeFactory.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<OliverContext>();

		var torrents = await db.TorrentInfos
			.Where(t => t.TorrentFileDownloaded && t.TorrentFilePath != null)
			.Select(t => new { t.Id, t.Hash, t.TorrentFilePath })
			.ToListAsync(ct);

		var result = new TorrentValidationResult { TotalChecked = torrents.Count };

		foreach (var torrent in torrents) {
			ct.ThrowIfCancellationRequested();

			if (!File.Exists(torrent.TorrentFilePath)) {
				result.MissingFiles.Add(torrent.Hash);
				continue;
			}

			var parsed = torrentParser.Parse(torrent.TorrentFilePath!);
			if (parsed is null) {
				result.CorruptFiles.Add(torrent.Hash);
			} else {
				result.ValidCount++;
			}
		}

		return result;
	}

	public async Task<ImageValidationResult> ValidateImageFilesAsync(CancellationToken ct = default) {
		using var scope = scopeFactory.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<OliverContext>();

		var movies = await db.Movies
			.Where(m => m.ImagesDownloaded)
			.Select(m => new { m.Id, m.YtsId })
			.ToListAsync(ct);

		var result = new ImageValidationResult { TotalMoviesChecked = movies.Count };

		foreach (var movie in movies) {
			ct.ThrowIfCancellationRequested();

			var movieDir = Path.Combine(_basePath, _imagesFolder, movie.YtsId.ToString(CultureInfo.InvariantCulture));
			if (!Directory.Exists(movieDir)) {
				result.MissingDirectories.Add(movie.YtsId);
				continue;
			}

			var files = Directory.GetFiles(movieDir);
			foreach (var file in files) {
				result.TotalFilesChecked++;
				if (!IsValidImage(file)) {
					result.CorruptFiles.Add(new CorruptImageEntry { YtsId = movie.YtsId, FilePath = file });
				} else {
					result.ValidCount++;
				}
			}
		}

		return result;
	}

	public async Task<GapReport> DetectGapsAsync(CancellationToken ct = default) {
		using var scope = scopeFactory.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<OliverContext>();

		var moviesWithoutTorrents = await db.Movies
			.Where(m => !m.Torrents.Any(t => t.TorrentFileDownloaded))
			.Select(m => new GapEntry { YtsId = m.YtsId, Title = m.Title })
			.ToListAsync(ct);

		var moviesWithoutImages = await db.Movies
			.Where(m => !m.ImagesDownloaded)
			.Select(m => new GapEntry { YtsId = m.YtsId, Title = m.Title })
			.ToListAsync(ct);

		var unparsedTorrents = await db.TorrentInfos
			.CountAsync(t => t.TorrentFileDownloaded && !t.TorrentFileParsed, ct);

		return new GapReport {
			MoviesWithoutTorrents = moviesWithoutTorrents,
			MoviesWithoutImages = moviesWithoutImages,
			UnparsedTorrentCount = unparsedTorrents,
		};
	}

	public async Task<CompletenessReport> GetCompletenessReportAsync(CancellationToken ct = default) {
		using var scope = scopeFactory.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<OliverContext>();

		var totalMovies = await db.Movies.CountAsync(ct);
		var totalTorrents = await db.TorrentInfos.CountAsync(ct);
		var torrentsDownloaded = await db.TorrentInfos.CountAsync(t => t.TorrentFileDownloaded, ct);
		var torrentsParsed = await db.TorrentInfos.CountAsync(t => t.TorrentFileParsed, ct);
		var imagesDownloaded = await db.Movies.CountAsync(m => m.ImagesDownloaded, ct);

		// Per-quality breakdown
		var qualityStats = await db.TorrentInfos
			.GroupBy(t => t.Quality)
			.Select(g => new QualityStat {
				Quality = g.Key ?? "unknown",
				Total = g.Count(),
				Downloaded = g.Count(t => t.TorrentFileDownloaded),
				Parsed = g.Count(t => t.TorrentFileParsed),
			})
			.OrderBy(q => q.Quality)
			.ToListAsync(ct);

		// Database file size
		var dbPath = configuration["Database:Path"] ?? "Data/oliver.db";
		var fullDbPath = Path.GetFullPath(dbPath);
		var dbSizeBytes = File.Exists(fullDbPath) ? new FileInfo(fullDbPath).Length : 0;

		return new CompletenessReport {
			TotalMovies = totalMovies,
			TotalTorrents = totalTorrents,
			TorrentsDownloaded = torrentsDownloaded,
			TorrentsParsed = torrentsParsed,
			ImagesDownloaded = imagesDownloaded,
			TorrentDownloadPercent = totalTorrents > 0 ? Math.Round(100.0 * torrentsDownloaded / totalTorrents, 1) : 0,
			TorrentParsePercent = torrentsDownloaded > 0 ? Math.Round(100.0 * torrentsParsed / torrentsDownloaded, 1) : 0,
			ImageDownloadPercent = totalMovies > 0 ? Math.Round(100.0 * imagesDownloaded / totalMovies, 1) : 0,
			QualityBreakdown = qualityStats,
			DatabaseSizeBytes = dbSizeBytes,
		};
	}

	private static bool IsValidImage(string filePath) {
		try {
			Span<byte> header = stackalloc byte[8];
			using var fs = File.OpenRead(filePath);
			var bytesRead = fs.Read(header);
			if (bytesRead < 3) {
				return false;
			}

			return header[..3].SequenceEqual(JpegMagic)
				|| (bytesRead >= 4 && header[..4].SequenceEqual(PngMagic));
		} catch {
			return false;
		}
	}
}

public class MovieCountVerification {
	public int DatabaseCount { get; set; }

	public int ApiTotalCount { get; set; }

	public int Missing { get; set; }

	public bool IsComplete { get; set; }
}

public class TorrentValidationResult {
	public int TotalChecked { get; set; }

	public int ValidCount { get; set; }

	public List<string> MissingFiles { get; set; } = [];

	public List<string> CorruptFiles { get; set; } = [];
}

public class ImageValidationResult {
	public int TotalMoviesChecked { get; set; }

	public int TotalFilesChecked { get; set; }

	public int ValidCount { get; set; }

	public List<int> MissingDirectories { get; set; } = [];

	public List<CorruptImageEntry> CorruptFiles { get; set; } = [];
}

public class CorruptImageEntry {
	public int YtsId { get; set; }

	public string FilePath { get; set; } = string.Empty;
}

public class GapReport {
	public List<GapEntry> MoviesWithoutTorrents { get; set; } = [];

	public List<GapEntry> MoviesWithoutImages { get; set; } = [];

	public int UnparsedTorrentCount { get; set; }
}

public class GapEntry {
	public int YtsId { get; set; }

	public string Title { get; set; } = string.Empty;
}

public class CompletenessReport {
	public int TotalMovies { get; set; }

	public int TotalTorrents { get; set; }

	public int TorrentsDownloaded { get; set; }

	public int TorrentsParsed { get; set; }

	public int ImagesDownloaded { get; set; }

	public double TorrentDownloadPercent { get; set; }

	public double TorrentParsePercent { get; set; }

	public double ImageDownloadPercent { get; set; }

	public List<QualityStat> QualityBreakdown { get; set; } = [];

	public long DatabaseSizeBytes { get; set; }
}

public class QualityStat {
	public string Quality { get; set; } = string.Empty;

	public int Total { get; set; }

	public int Downloaded { get; set; }

	public int Parsed { get; set; }
}
