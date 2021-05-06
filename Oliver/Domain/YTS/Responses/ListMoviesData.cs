using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Oliver.Domain.YTS.Responses {
	public class ListMoviesData {
		[JsonPropertyName("movie_count")]
		public int MovieCount { get; set; }

		[JsonPropertyName("limit")]
		public int Limit { get; set; }

		[JsonPropertyName("page_number")]
		public int PageNumber { get; set; }

		[JsonPropertyName("movies")]
		public List<YtsMovie> Movies { get; set; }
	}
}
