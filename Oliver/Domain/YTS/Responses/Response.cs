using System.Text.Json.Serialization;

namespace Oliver.Domain.YTS.Responses {
	public class Response<T> where T : class {
		// Can be "ok" or "error"
		[JsonPropertyName("status")]
		public string Status { get; set; }

		public bool IsOK { get => Status == "ok"; }

		[JsonPropertyName("status_message")]
		public string StatusMessage { get; set; }

		[JsonPropertyName("data")]
		public T Data { get; set; }

		[JsonPropertyName("@meta")]
		public ResponseMeta Meta { get; set; }
	}
}
