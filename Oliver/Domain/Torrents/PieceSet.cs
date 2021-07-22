using System;
using System.Collections.Generic;
using System.Linq;
using Oliver.Extensions;

namespace Oliver.Domain.Torrents {
	public class PieceSet {
		public PieceSize Size { get; }

		public List<PieceHash> Pieces { get; }

		public byte[] FullHash => Pieces.Select(x => x.Hash).JoinArrays();

		public string FullHashString => FullHash.ToHexString();

		public PieceSet(PieceSize size) {
			Size = size;
			Pieces = new();
		}

		public PieceSet(PieceSize size, int numberOfPieces) {
			Size = size;
			Pieces = new(numberOfPieces);
		}

		public PieceSet(PieceSize size, List<PieceHash> pieces) {
			Size = size;
			Pieces = pieces;
		}

		public PieceSet(PieceSize size, byte[] data) {
			if (data == null) {
				throw new ArgumentNullException(nameof(data));
			}

			if (data.Length % PieceHash.HashLength != 0) {
				throw new ArgumentException($"{nameof(data)} must have a length that is a multiple of {PieceHash.HashLength}", nameof(data));
			}

			Pieces = new(data.Length / PieceHash.HashLength);

			for (var offset = 0; offset <= data.Length; offset += PieceHash.HashLength) {
				Pieces.Add(new PieceHash(data.AsSpan(offset, PieceHash.HashLength).ToArray()));
			}

			Size = size;
		}
	}
}
