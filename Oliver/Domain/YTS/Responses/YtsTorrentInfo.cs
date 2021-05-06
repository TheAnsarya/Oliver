using System.Text.Json.Serialization;

namespace Oliver.Domain.YTS.Responses {
	public class YtsTorrentInfo {
		[JsonPropertyName("url")]
		public string Url { get; set; }

		[JsonPropertyName("hash")]
		public string Hash { get; set; }

		[JsonPropertyName("quality")]
		public string Quality { get; set; }

		[JsonPropertyName("type")]
		public string Type { get; set; }

		[JsonPropertyName("seeds")]
		public int Seeds { get; set; }

		[JsonPropertyName("peers")]
		public int Peers { get; set; }

		[JsonPropertyName("size")]
		public string Size { get; set; }

		[JsonPropertyName("size_bytes")]
		public long SizeBytes { get; set; }

		[JsonPropertyName("date_uploaded")]
		public string DateUploaded { get; set; }

		[JsonPropertyName("date_uploaded_unix")]
		public long DateUploadedUnix { get; set; }
	}
}
