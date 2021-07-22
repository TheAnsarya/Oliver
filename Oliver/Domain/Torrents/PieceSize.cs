using System;

namespace Oliver.Domain.Torrents {
	public class PieceSize {
		// (pieceSize % PieceSizeDivider == 0) should be true
		public const int PieceSizeDivider = 16 * 1024;

		private int _size;

		public int Size {
			get => _size;
			set {
				if (value <= 0) {
					throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Size)} must be greater than zero.");
				} else if (value % PieceSizeDivider != 0) {
					// TODO: if this becomes a problem with the yts torrents, change or add exceptions (log but proceed in actions?)
					throw new ArgumentException($"{nameof(Size)} must be a multiple of 16 kb.", nameof(value));
				}

				_size = value;
			}
		}

		// You can deal with the piece size as a long because it is in the definition of a torrent and some apis use it as such,
		// but it is really an int here as larger sizes for pieces are ridiculous
		public long SizeLong {
			get => _size;
			set {
				if (value <= 0) {
					throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(SizeLong)} must be greater than zero.");
				} else if (value % PieceSizeDivider != 0) {
					// TODO: if this becomes a problem with the yts torrents, change or add exceptions (log but proceed in actions?)
					throw new ArgumentException($"{nameof(SizeLong)} must be a multiple of 16 kb.", nameof(value));
				} else if (value > int.MaxValue) {
					throw new ArgumentOutOfRangeException(nameof(value),
						$"{nameof(SizeLong)} must be within the range of integers, so less than or equal to {int.MaxValue}."
						+ " If you see this exception you are doing it wrong; pieces should be much smaller than this.");
				}

				_size = (int)value;
			}
		}

		public PieceSize(int size) => Size = size;

		public PieceSize(long size) => SizeLong = size;

		// How many pieces of this size will it take to cover data of the given length
		public int ExpectedPieces(long totalSize) => (int)Math.Ceiling(totalSize / (double)Size);
	}
}
