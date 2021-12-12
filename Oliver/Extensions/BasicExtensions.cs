using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Oliver.Extensions {
	public static class BasicExtensions {
		public static string ToHexString(this byte[] data) {
			if (data == null) {
				throw new ArgumentNullException(nameof(data), "Cannot convert null argument.");
			}

			var sb = new StringBuilder(data.Length * 2);

			foreach (var value in data) {
				_ = sb.Append(value.ToString("x2", CultureInfo.InvariantCulture));
			}

			var hex = sb.ToString();

			return hex;
		}

		public static bool IsSame(this ReadOnlySpan<byte> main, ReadOnlySpan<byte> other) => main.SequenceEqual(other);

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
		// TODO: this enumerates twice which could be a problem
		public static T[] JoinArrays<T>(this IEnumerable<T[]> arrays) {
			if (arrays == null) {
				throw new ArgumentNullException(nameof(arrays));
			}

			var count = arrays.Sum(x => x?.Length ?? 0);

			var joined = new T[count];

			var index = 0;

			foreach (var item in arrays) {
				if (item != null) {
					Array.Copy(item, 0, joined, index, item.Length);
					index += item.Length;
				}
			}

			return joined;
		}
	}
}
