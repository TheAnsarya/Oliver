using System.Globalization;

namespace Oliver.Services;

public class DownloadService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<DownloadService> logger) {
	private readonly HttpClient _httpClient = httpClientFactory.CreateClient("yts");
	private readonly ILogger<DownloadService> _logger = logger;
	private readonly string _basePath = Path.GetFullPath(configuration["Downloads:BasePath"] ?? "YtsData");
	private readonly string _torrentsFolder = configuration["Downloads:TorrentsFolder"] ?? "torrents";
	private readonly string _imagesFolder = configuration["Downloads:ImagesFolder"] ?? "images";
	private readonly int _maxRetries = configuration.GetValue("Downloads:MaxRetries", 3);

	public void EnsureDirectories() {
		Directory.CreateDirectory(Path.Combine(_basePath, _torrentsFolder));
		Directory.CreateDirectory(Path.Combine(_basePath, _imagesFolder));
	}

	public async Task<string?> DownloadTorrentAsync(string hash, Uri url, CancellationToken ct = default) {
		var fileName = $"{hash}.torrent";
		var filePath = Path.Combine(_basePath, _torrentsFolder, fileName);

		if (File.Exists(filePath)) {
			return filePath;
		}

		for (var attempt = 1; attempt <= _maxRetries; attempt++) {
			try {
				var data = await _httpClient.GetByteArrayAsync(url, ct);
				await File.WriteAllBytesAsync(filePath, data, ct);
				_logger.LogDebug("Downloaded torrent {Hash}", hash);
				return filePath;
			} catch (OperationCanceledException) {
				throw;
			} catch (Exception ex) when (attempt < _maxRetries) {
				var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
				_logger.LogWarning(ex, "Failed to download torrent {Hash} (attempt {Attempt}/{Max}), retrying in {Delay}s",
					hash, attempt, _maxRetries, delay.TotalSeconds);
				await Task.Delay(delay, ct);
			} catch (Exception ex) {
				_logger.LogWarning(ex, "Failed to download torrent {Hash} from {Url} after {Max} attempts",
					hash, url, _maxRetries);
				return null;
			}
		}

		return null;
	}

	public async Task DownloadMovieImagesAsync(int ytsId, Dictionary<string, string?> imageUrls, CancellationToken ct = default) {
		ArgumentNullException.ThrowIfNull(imageUrls);
		var movieDir = Path.Combine(_basePath, _imagesFolder, ytsId.ToString(CultureInfo.InvariantCulture));
		Directory.CreateDirectory(movieDir);

		foreach (var (name, url) in imageUrls) {
			if (string.IsNullOrEmpty(url)) {
				continue;
			}

			var extension = GetImageExtension(url);
			var filePath = Path.Combine(movieDir, $"{name}{extension}");

			if (File.Exists(filePath)) {
				continue;
			}

			for (var attempt = 1; attempt <= _maxRetries; attempt++) {
				try {
					var data = await _httpClient.GetByteArrayAsync(url, ct);
					await File.WriteAllBytesAsync(filePath, data, ct);
					break;
				} catch (OperationCanceledException) {
					throw;
				} catch (Exception ex) when (attempt < _maxRetries) {
					var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
					_logger.LogWarning(ex, "Failed to download image {Name} for movie {YtsId} (attempt {Attempt}/{Max}), retrying",
						name, ytsId, attempt, _maxRetries);
					await Task.Delay(delay, ct);
				} catch (Exception ex) {
					_logger.LogWarning(ex, "Failed to download image {Name} for movie {YtsId} after {Max} attempts",
						name, ytsId, _maxRetries);
				}
			}
		}
	}

	private static string GetImageExtension(string url) {
		if (url.Contains(".png", StringComparison.OrdinalIgnoreCase)) {
			return ".png";
		}
		return ".jpg";
	}
}
