using Microsoft.EntityFrameworkCore;
using Oliver.BackgroundServices;
using Oliver.Data;
using Oliver.Services;
using Serilog;

#pragma warning disable CA1305 // Serilog static configuration doesn't accept IFormatProvider
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Information()
	.WriteTo.Console()
	.WriteTo.File("logs/oliver-.txt", rollingInterval: RollingInterval.Day)
	.CreateLogger();
#pragma warning restore CA1305

try {
	Log.Information("Oliver starting up");

	var builder = WebApplication.CreateBuilder(args);
	builder.Services.AddSerilog();

	// Database
	var dbPath = builder.Configuration["Database:Path"] ?? "Data/oliver.db";
	var fullDbPath = Path.GetFullPath(dbPath);
	var dbDir = Path.GetDirectoryName(fullDbPath);
	if (dbDir is not null && !Directory.Exists(dbDir)) {
		Directory.CreateDirectory(dbDir);
	}
	builder.Services.AddDbContext<OliverContext>(options =>
		options.UseSqlite($"Data Source={fullDbPath}"));

	// HTTP client with connection pooling
	builder.Services.AddHttpClient("yts", client => {
		client.DefaultRequestHeaders.UserAgent.ParseAdd("Oliver/2.0");
		client.Timeout = TimeSpan.FromSeconds(30);
	}).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler {
		PooledConnectionLifetime = TimeSpan.FromMinutes(5),
		MaxConnectionsPerServer = 10,
	});

	// Services
	builder.Services.AddScoped<YtsApiClient>();
	builder.Services.AddScoped<DownloadService>();
	builder.Services.AddSingleton<TorrentParsingService>();
	builder.Services.AddSingleton<DataVerificationService>();

	// CORS for UI dev server
	builder.Services.AddCors(options => {
		options.AddDefaultPolicy(policy => {
			policy.WithOrigins("http://localhost:3000")
				.AllowAnyHeader()
				.AllowAnyMethod();
		});
	});

	// Background worker
	builder.Services.AddHostedService<YtsSyncWorker>();

	var app = builder.Build();

	// Ensure database is created
	using (var scope = app.Services.CreateScope()) {
		var db = scope.ServiceProvider.GetRequiredService<OliverContext>();
		await db.Database.EnsureCreatedAsync();
		Log.Information("Database ready at {Path}", fullDbPath);
	}

	app.UseCors();

	// API endpoints
	var api = app.MapGroup("/api");

	api.MapGet("/stats", async (OliverContext db) => {
		var movieCount = await db.Movies.CountAsync();
		var torrentCount = await db.TorrentInfos.CountAsync();
		var torrentsDownloaded = await db.TorrentInfos.CountAsync(t => t.TorrentFileDownloaded);
		var torrentsParsed = await db.TorrentInfos.CountAsync(t => t.TorrentFileParsed);
		var imagesDownloaded = await db.Movies.CountAsync(m => m.ImagesDownloaded);
		var genreCount = await db.Genres.Select(g => g.Name).Distinct().CountAsync();
		var fileEntryCount = await db.TorrentFileEntries.CountAsync();
		var trackerCount = await db.TorrentTrackers.Select(t => t.Url).Distinct().CountAsync();

		return Results.Ok(new {
			movies = movieCount,
			torrents = torrentCount,
			torrentsDownloaded,
			torrentsParsed,
			imagesDownloaded,
			genres = genreCount,
			torrentFiles = fileEntryCount,
			uniqueTrackers = trackerCount,
		});
	});

	api.MapGet("/movies", async (OliverContext db, string? search, string? genre, string? quality, int page = 1, int limit = 20) => {
		limit = Math.Clamp(limit, 1, 100);
		page = Math.Max(1, page);

		var query = db.Movies
			.Include(m => m.Genres)
			.Include(m => m.Torrents)
			.AsQueryable();

		if (!string.IsNullOrWhiteSpace(search)) {
			var term = search.Trim();
			query = query.Where(m => m.Title.Contains(term) || (m.ImdbCode != null && m.ImdbCode.Contains(term)));
		}

		if (!string.IsNullOrWhiteSpace(genre)) {
			query = query.Where(m => m.Genres.Any(g => g.Name == genre));
		}

		if (!string.IsNullOrWhiteSpace(quality)) {
			query = query.Where(m => m.Torrents.Any(t => t.Quality == quality));
		}

		var totalCount = await query.CountAsync();
		var movies = await query
			.OrderByDescending(m => m.Year)
			.ThenByDescending(m => m.Rating)
			.Skip((page - 1) * limit)
			.Take(limit)
			.Select(m => new {
				m.Id,
				m.YtsId,
				m.Title,
				m.Year,
				m.Rating,
				m.Runtime,
				m.ImdbCode,
				m.MediumCoverImage,
				m.LargeCoverImage,
				m.Language,
				m.ImagesDownloaded,
				genres = m.Genres.Select(g => g.Name).ToList(),
				torrents = m.Torrents.Select(t => new {
					t.Quality,
					t.Type,
					t.Size,
					t.SizeBytes,
					t.Seeds,
					t.Peers,
					t.TorrentFileDownloaded,
				}).ToList(),
			})
			.ToListAsync();

		return Results.Ok(new {
			movies,
			totalCount,
			page,
			limit,
			totalPages = (int)Math.Ceiling((double)totalCount / limit),
		});
	});

	api.MapGet("/sync-status", async (OliverContext db) => {
		var states = await db.SyncStates.ToListAsync();
		var lastPage = states.FirstOrDefault(s => s.Key == "LastCompletedPage");

		return Results.Ok(new {
			lastCompletedPage = lastPage?.Value ?? "0",
			syncStates = states.Select(s => new { s.Key, s.Value }).ToList(),
		});
	});

	api.MapGet("/genres", async (OliverContext db) => {
		var genres = await db.Genres
			.GroupBy(g => g.Name)
			.Select(g => new { name = g.Key, count = g.Count() })
			.OrderBy(g => g.name)
			.ToListAsync();

		return Results.Ok(genres);
	});

	// Verification endpoints
	var verify = api.MapGroup("/verify");

	verify.MapGet("/movie-count", async (DataVerificationService svc, CancellationToken ct) =>
		Results.Ok(await svc.VerifyMovieCountAsync(ct)));

	verify.MapGet("/torrents", async (DataVerificationService svc, CancellationToken ct) =>
		Results.Ok(await svc.ValidateTorrentFilesAsync(ct)));

	verify.MapGet("/images", async (DataVerificationService svc, CancellationToken ct) =>
		Results.Ok(await svc.ValidateImageFilesAsync(ct)));

	verify.MapGet("/gaps", async (DataVerificationService svc, CancellationToken ct) =>
		Results.Ok(await svc.DetectGapsAsync(ct)));

	verify.MapGet("/completeness", async (DataVerificationService svc, CancellationToken ct) =>
		Results.Ok(await svc.GetCompletenessReportAsync(ct)));

	await app.RunAsync();
} catch (Exception ex) {
	Log.Fatal(ex, "Oliver terminated unexpectedly");
} finally {
	await Log.CloseAndFlushAsync();
}
