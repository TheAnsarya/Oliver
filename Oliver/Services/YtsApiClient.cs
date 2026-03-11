using System.Text.Json;
using Oliver.Domain.YTS;

namespace Oliver.Services;

public class YtsApiClient {
	private readonly HttpClient _httpClient;
	private readonly ILogger<YtsApiClient> _logger;
	private readonly string _baseUrl;

	public YtsApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<YtsApiClient> logger) {
		ArgumentNullException.ThrowIfNull(httpClientFactory);
		ArgumentNullException.ThrowIfNull(configuration);
		_httpClient = httpClientFactory.CreateClient("yts");
		_logger = logger;
		_baseUrl = configuration["Yts:ApiBaseUrl"] ?? "https://yts.mx/api/v2/";
	}

	public async Task<(List<YtsMovie> Movies, int TotalCount)> GetMoviesPageAsync(int page, int limit = 50, CancellationToken ct = default) {
		var url = $"{_baseUrl}list_movies.json?page={page}&limit={limit}&sort_by=date_added&order_by=asc";
		_logger.LogDebug("Fetching page {Page}: {Url}", page, url);

		var json = await _httpClient.GetStringAsync(url, ct);
		var response = JsonSerializer.Deserialize<YtsResponse<ListMoviesData>>(json);

		if (response is null || !response.IsOk || response.Data?.Movies is null) {
			_logger.LogWarning("API returned non-ok or empty response for page {Page}", page);
			return ([], 0);
		}

		return (response.Data.Movies, response.Data.MovieCount);
	}
}
