using System.Globalization;

namespace Oliver.Services;

public class DownloadService {
	private readonly HttpClient _httpClient;
	private readonly ILogger<DownloadService> _logger;
	private readonly string _basePath;
	private readonly string _torrentsFolder;
	private readonly string _imagesFolder;

	public DownloadService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<DownloadService> logger) {
		ArgumentNullException.ThrowIfNull(httpClientFactory);
		ArgumentNullException.ThrowIfNull(configuration);
		_httpClient = httpClientFactory.CreateClient("yts");
		_logger = logger;
		_basePath = Path.GetFullPath(configuration["Downloads:BasePath"] ?? "YtsData");
		_torrentsFolder = configuration["Downloads:TorrentsFolder"] ?? "torrents";
		_imagesFolder = configuration["Downloads:ImagesFolder"] ?? "images";

		Directory.CreateDirectory(Path.Combine(_basePath, _torrentsFolder));
		Directory.CreateDirectory(Path.Combine(_basePath, _imagesFolder));
	}

	public async Task<string?> DownloadTorrentAsync(string hash, Uri url, CancellationToken ct = default) {
		var fileName = $"{hash}.torrent";
		var filePath = Path.Combine(_basePath, _torrentsFolder, fileName);

		if (File.Exists(filePath)) {
			return filePath;
		}

		try {
			var data = await _httpClient.GetByteArrayAsync(url, ct);
			await File.WriteAllBytesAsync(filePath, data, ct);
			_logger.LogDebug("Downloaded torrent {Hash}", hash);
			return filePath;
		} catch (Exception ex) {
			_logger.LogWarning(ex, "Failed to download torrent {Hash} from {Url}", hash, url);
			return null;
		}
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

			try {
				var data = await _httpClient.GetByteArrayAsync(url, ct);
				await File.WriteAllBytesAsync(filePath, data, ct);
			} catch (Exception ex) {
				_logger.LogWarning(ex, "Failed to download image {Name} for movie {YtsId}", name, ytsId);
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
