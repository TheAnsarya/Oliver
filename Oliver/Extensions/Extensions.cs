using System;
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

		public static bool IsSame(this ReadOnlySpan<byte> main, ReadOnlySpan<byte> other) {
			return main.SequenceEqual(other);
		}

		public static bool IsSame(this byte[] main, ReadOnlySpan<byte> other) {
			var span = (ReadOnlySpan<byte>)main;
			return span.SequenceEqual(other);
		}

		public static string Timestamp(this DateTime date) => date.ToString("yyyy-MM-dd_HH-mm-ss-ffff");
	}
}
