using System.Text.Json;
using Oliver.Domain.YTS;

namespace Oliver.Services;

public class YtsApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<YtsApiClient> logger) {
	private readonly HttpClient _httpClient = httpClientFactory.CreateClient("yts");
	private readonly ILogger<YtsApiClient> _logger = logger;
	private readonly string _baseUrl = configuration["Yts:ApiBaseUrl"] ?? "https://yts.mx/api/v2/";
	private readonly int _maxRetries = configuration.GetValue("Yts:MaxRetries", 3);

	public async Task<(List<YtsMovie> Movies, int TotalCount)> GetMoviesPageAsync(int page, int limit = 50, CancellationToken ct = default) {
		var url = $"{_baseUrl}list_movies.json?page={page}&limit={limit}&sort_by=date_added&order_by=asc";
		_logger.LogDebug("Fetching page {Page}: {Url}", page, url);

		for (var attempt = 1; attempt <= _maxRetries; attempt++) {
			try {
				var json = await _httpClient.GetStringAsync(url, ct);
				var response = JsonSerializer.Deserialize<YtsResponse<ListMoviesData>>(json);

				if (response is null || !response.IsOk || response.Data?.Movies is null) {
					_logger.LogWarning("API returned non-ok or empty response for page {Page}", page);
					return ([], 0);
				}

				return (response.Data.Movies, response.Data.MovieCount);
			} catch (OperationCanceledException) {
				throw;
			} catch (HttpRequestException ex) when (attempt < _maxRetries) {
				var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
				_logger.LogWarning(ex, "API request failed for page {Page} (attempt {Attempt}/{Max}), retrying in {Delay}s",
					page, attempt, _maxRetries, delay.TotalSeconds);
				await Task.Delay(delay, ct);
			}
		}

		_logger.LogError("API request failed for page {Page} after {Max} attempts", page, _maxRetries);
		return ([], 0);
	}
}
