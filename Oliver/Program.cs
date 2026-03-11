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

	var builder = Host.CreateApplicationBuilder(args);
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

	// HTTP client
	builder.Services.AddHttpClient("yts", client => {
		client.DefaultRequestHeaders.UserAgent.ParseAdd("Oliver/1.0");
		client.Timeout = TimeSpan.FromSeconds(30);
	});

	// Services
	builder.Services.AddScoped<YtsApiClient>();
	builder.Services.AddScoped<DownloadService>();

	// Background worker
	builder.Services.AddHostedService<YtsSyncWorker>();

	var host = builder.Build();

	// Ensure database is created
	using (var scope = host.Services.CreateScope()) {
		var db = scope.ServiceProvider.GetRequiredService<OliverContext>();
		await db.Database.EnsureCreatedAsync();
		Log.Information("Database ready at {Path}", fullDbPath);
	}

	await host.RunAsync();
} catch (Exception ex) {
	Log.Fatal(ex, "Oliver terminated unexpectedly");
} finally {
	await Log.CloseAndFlushAsync();
}
