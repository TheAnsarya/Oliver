using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Oliver.Domain.YTS.Responses
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "DTO class")]
	public class ListMoviesData
	{
		[JsonPropertyName("movie_count")]
		public int MovieCount { get; set; }

		[JsonPropertyName("limit")]
		public int Limit { get; set; }

		[JsonPropertyName("page_number")]
		public int PageNumber { get; set; }

		[JsonPropertyName("movies")]
		public List<Movie> Movies { get; set; }
	}
}
