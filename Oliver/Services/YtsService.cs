using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Oliver.Domain.YTS.Requests;
using Oliver.Domain.YTS.Responses;
using Oliver.Services.Interfaces;

namespace Oliver.Services {
	public class YtsService : IYtsService {
		private readonly IConfiguration _config;

		private readonly IHttpClientFactory _clientFactory;

		public YtsService(IConfiguration config, IHttpClientFactory clientFactory) {
			_config = config;
			_clientFactory = clientFactory;
		}

		public async Task<Response<ListMoviesData>> FetchMoviesList(ListMoviesRequest request = null) {
			if (request == null) {
				request = new ListMoviesRequest();
			}

			var baseAddress = _config["Yts:BaseAddress"];
			var endpoint = $"{baseAddress}api/v2/list_movies.json";
			var url = $"{endpoint}{request.QueryString()}";

			var client = _clientFactory.CreateClient();
			var response = await client.GetStringAsync(url);

			var moviesList = JsonSerializer.Deserialize<Response<ListMoviesData>>(response);

			return moviesList;
		}
	}
}
