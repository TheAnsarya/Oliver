using Microsoft.Extensions.Configuration;
using Oliver.Domain.YTS.Requests;
using Oliver.Domain.YTS.Responses;
using Oliver.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Oliver.Services
{
	public class YtsService : IYtsService
	{
		private readonly IConfiguration _config;

		private readonly IHttpClientFactory _clientFactory;

		public YtsService(IConfiguration config, IHttpClientFactory clientFactory)
		{
			_config = config;
			_clientFactory = clientFactory;
		}

		public async Task<Response<ListMoviesData>> FetchMoviesList(ListMoviesRequest request = null)
		{
			if (request == null)
			{
				request = new ListMoviesRequest();
			}

			var baseAddress = _config["Yts:BaseAddress"];
			var endpoint = new Uri($"{baseAddress}/api/v2/list_movies.json");
			var url = $"{endpoint}{request.QueryString()}";

			var requestMessage = new HttpRequestMessage()
			{
				RequestUri = new Uri(url),
				Method = HttpMethod.Get
			};

			requestMessage.Headers.Accept.Clear();
			requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			var client = _clientFactory.CreateClient();
			using var response = await client.SendAsync(requestMessage);
			response.EnsureSuccessStatusCode();
			var responseContent = await response.Content.ReadAsStringAsync();

			var moviesList = JsonSerializer.Deserialize<Response<ListMoviesData>>(responseContent);

			return moviesList;
		}
	}
}
