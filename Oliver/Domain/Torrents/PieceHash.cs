using System;
using Oliver.Extensions;

namespace Oliver.Domain.Torrents {
	public class PieceHash {
		public const int HashLength = 20;
		public const int HashStringLength = HashLength * 2;

		private byte[] _hash;
		private string _hashString;

		public byte[] Hash {
			get => _hash;
			set {
				if (value == null) {
					throw new ArgumentNullException(nameof(value), $"{nameof(Hash)} cannot be null.");
				} else if (value.Length != HashLength) {
					throw new ArgumentException($"{nameof(Hash)} must be {HashLength} bytes.", nameof(value));
				}

				_hashString = null;
				_hash = value;
			}
		}

		// TODO: Figure out why this didn't work: return _hashString ?? (_hashString = _hash.ToHexString());
		public string HashString {
			get {
				if (_hashString == null) {
					_hashString = _hash.ToHexString();
				}

				return _hashString;
			}
		}

		public PieceHash(byte[] hash) => Hash = hash;
	}
}
