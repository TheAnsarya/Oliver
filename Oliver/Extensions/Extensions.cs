using System.Text;

namespace Oliver.Extensions {
	public static class Extensions {
		public static string ToHexString(this byte[] data) {
			var sb = new StringBuilder();

			foreach (var value in data) {
				sb.Append(value.ToString("x2"));
			}

			string hex = sb.ToString();

			return hex;
		}
	}
}
