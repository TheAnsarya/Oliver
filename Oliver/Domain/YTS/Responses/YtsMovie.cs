using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Oliver.Domain.YTS.Responses {
	public class YtsMovie {
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("url")]
		public string Url { get; set; }

		[JsonPropertyName("imdb_code")]
		public string ImdbCode { get; set; }

		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("title_english")]
		public string TitleEnglish { get; set; }

		[JsonPropertyName("title_long")]
		public string TitleLong { get; set; }

		[JsonPropertyName("slug")]
		public string Slug { get; set; }

		[JsonPropertyName("year")]
		public int Year { get; set; }

		[JsonPropertyName("rating")]
		public decimal Rating { get; set; }

		[JsonPropertyName("runtime")]
		public int Runtime { get; set; }

		[JsonPropertyName("genres")]
		public List<string> Genres { get; set; }

		[JsonPropertyName("summary")]
		public string Summary { get; set; }

		[JsonPropertyName("description_full")]
		public string DescriptionFull { get; set; }

		[JsonPropertyName("synopsis")]
		public string Synopsis { get; set; }

		[JsonPropertyName("yt_trailer_code")]
		public string YtTrailerCode { get; set; }

		[JsonPropertyName("language")]
		public string Language { get; set; }

		[JsonPropertyName("mpa_rating")]
		public string MpaRating { get; set; }

		[JsonPropertyName("background_image")]
		public string BackgroundImage { get; set; }

		[JsonPropertyName("background_image_original")]
		public string BackgroundImageOriginal { get; set; }

		[JsonPropertyName("small_cover_image")]
		public string SmallCoverImage { get; set; }

		[JsonPropertyName("medium_cover_image")]
		public string MediumCoverImage { get; set; }

		[JsonPropertyName("large_cover_image")]
		public string LargeCoverImage { get; set; }

		[JsonPropertyName("state")]
		public string State { get; set; }

		[JsonPropertyName("torrents")]
		public List<YtsTorrentInfo> Torrents { get; set; }

		[JsonPropertyName("date_uploaded")]
		public string DateUploaded { get; set; }

		[JsonPropertyName("date_uploaded_unix")]
		public long DateUploadedUnix { get; set; }
	}
}
