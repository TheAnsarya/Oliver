using System;
using System.Collections.Generic;
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

		public static IEnumerable<string> Split(this string input, int length) {
			if (input == null) {
				throw new ArgumentNullException(nameof(input));
			} else if (length < 1) { 
				throw new ArgumentOutOfRangeException("Length must be greater than zero.", nameof(length));
			}

			for (int i = 0; i < input.Length; i += length) {
				yield return input.Substring(i, Math.Min(length, input.Length - i));
			}
		}
	}
}
