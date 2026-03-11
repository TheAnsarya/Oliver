using Microsoft.EntityFrameworkCore;
using Oliver.Data;
using Oliver.Domain;

namespace Oliver.Services;

public class FileScannerService(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<FileScannerService> logger) {
	private static readonly HashSet<string> VideoExtensions = new(StringComparer.OrdinalIgnoreCase) {
		".mkv", ".mp4", ".avi", ".mov", ".wmv", ".flv", ".webm", ".m4v", ".mpg", ".mpeg",
	};

	public async Task<FileScanResult> ScanAsync(CancellationToken ct = default) {
		var scanPaths = configuration.GetSection("FileScan:Paths").Get<string[]>() ?? [];
		if (scanPaths.Length == 0) {
			logger.LogInformation("No scan paths configured in FileScan:Paths");
			return new FileScanResult();
		}

		using var scope = scopeFactory.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<OliverContext>();

		var knownPaths = await db.LocalFiles.Select(f => f.FilePath).ToHashSetAsync(ct);
		var result = new FileScanResult();

		foreach (var scanPath in scanPaths) {
			ct.ThrowIfCancellationRequested();
			var fullPath = Path.GetFullPath(scanPath);
			if (!Directory.Exists(fullPath)) {
				logger.LogWarning("Scan path does not exist: {Path}", fullPath);
				continue;
			}

			result.DirectoriesScanned++;
			logger.LogInformation("Scanning {Path}", fullPath);

			var files = Directory.EnumerateFiles(fullPath, "*", new EnumerationOptions {
				RecurseSubdirectories = true,
				IgnoreInaccessible = true,
			});

			var batch = new List<LocalFile>();

			foreach (var file in files) {
				ct.ThrowIfCancellationRequested();

				var ext = Path.GetExtension(file);
				if (!VideoExtensions.Contains(ext)) {
					continue;
				}

				result.TotalFilesFound++;

				if (knownPaths.Contains(file)) {
					result.AlreadyKnown++;
					continue;
				}

				var info = new FileInfo(file);
				batch.Add(new LocalFile {
					FilePath = file,
					FileName = Path.GetFileNameWithoutExtension(file),
					Extension = ext,
					FileSize = info.Length,
					FileModified = info.LastWriteTimeUtc,
				});

				if (batch.Count >= 100) {
					db.LocalFiles.AddRange(batch);
					await db.SaveChangesAsync(ct);
					result.NewFilesAdded += batch.Count;
					batch.Clear();
				}
			}

			if (batch.Count > 0) {
				db.LocalFiles.AddRange(batch);
				await db.SaveChangesAsync(ct);
				result.NewFilesAdded += batch.Count;
			}
		}

		logger.LogInformation("File scan complete: {Found} found, {New} new, {Known} already known",
			result.TotalFilesFound, result.NewFilesAdded, result.AlreadyKnown);

		return result;
	}
}

public class FileScanResult {
	public int DirectoriesScanned { get; set; }

	public int TotalFilesFound { get; set; }

	public int NewFilesAdded { get; set; }

	public int AlreadyKnown { get; set; }
}
