using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Oliver.Extensions {
	public static class Extensions {
		public static string ToHexString(this byte[] data) {
			if (data == null) {
				throw new ArgumentNullException(nameof(data), "Connot convert null argument.");
			}

			var sb = new StringBuilder();

			foreach (var value in data) {
				_ = sb.Append(value.ToString("x2", CultureInfo.InvariantCulture));
			}

			var hex = sb.ToString();

			return hex;
		}

		public static bool IsSame(this ReadOnlySpan<byte> main, ReadOnlySpan<byte> other) {
			return main.SequenceEqual(other);
		}

		public static bool IsSame(this byte[] main, ReadOnlySpan<byte> other) {
			var span = (ReadOnlySpan<byte>)main;
			return span.SequenceEqual(other);
		}

		public static string Timestamp(this DateTime date) => date.ToString("yyyy-MM-dd_HH-mm-ss-ffff", CultureInfo.InvariantCulture);

		public static IEnumerable<string> Split(this string input, int length) {
			if (input == null) {
				throw new ArgumentNullException(nameof(input));
			} else if (length < 1) {
				throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than zero.");
			}

			for (var i = 0; i < input.Length; i += length) {
				yield return input.Substring(i, Math.Min(length, input.Length - i));
			}
		}

		// TODO: fix this, is not setup as we do things
		public static T[] JoinArrays<T>(this IEnumerable<T[]> self) {
			if (self == null) {
				throw new ArgumentNullException(nameof(self));
			}

			var count = 0;

			foreach (var arr in self) {
				if (arr != null) {
					count += arr.Length;
				}
			}

			var joined = new T[count];

			var index = 0;

			foreach (var arr in self) {
				if (arr != null) {
					Array.Copy(arr, 0, joined, index, arr.Length);
					index += arr.Length;
				}
			}

			return joined;
		}
	}
}
