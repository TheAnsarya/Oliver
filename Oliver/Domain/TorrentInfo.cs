using System;
using System.Collections.Generic;
using Oliver.Domain.YTS.Responses;

namespace Oliver.Domain {
	public class TorrentInfo : Entity, IEquatable<TorrentInfo> {
		public string Url { get; set; }

		public string Hash { get; set; }

		public string Quality { get; set; }

		public string Type { get; set; }

		public string Size { get; set; }

		public long SizeBytes { get; set; }

		public string DateUploaded { get; set; }

		public long DateUploadedUnix { get; set; }

		public bool Current { get; set; }

		// Link Properties

		public virtual Movie Movie { get; set; }

		public virtual TorrentFile TorrentFile { get; set; }

		// Constructors

		public TorrentInfo() : base() { }

		public TorrentInfo(YtsTorrentInfo dto, Movie movie) {
			Url = dto.Url;
			Hash = dto.Hash;
			Quality = dto.Quality;
			Type = dto.Type;
			Size = dto.Size;
			SizeBytes = dto.SizeBytes;
			DateUploaded = dto.DateUploaded;
			DateUploadedUnix = dto.DateUploadedUnix;

			Movie = movie;
			Current = true;
		}

		// Equality

		public override bool Equals(object obj) {
			return Equals(obj as TorrentInfo);
		}

		public bool Equals(TorrentInfo other) {
			return other != null &&
				   Url == other.Url &&
				   Hash == other.Hash &&
				   Quality == other.Quality &&
				   Type == other.Type &&
				   Size == other.Size &&
				   SizeBytes == other.SizeBytes &&
				   DateUploaded == other.DateUploaded &&
				   DateUploadedUnix == other.DateUploadedUnix;
		}

		public override int GetHashCode() {
			return HashCode.Combine(Url, Hash, Quality, Type, Size, SizeBytes, DateUploaded, DateUploadedUnix);
		}

		public static bool operator ==(TorrentInfo left, TorrentInfo right) {
			return EqualityComparer<TorrentInfo>.Default.Equals(left, right);
		}

		public static bool operator !=(TorrentInfo left, TorrentInfo right) {
			return !(left == right);
		}

		// Update

		public void Update(TorrentInfo other) {
			if (other == null) {
				throw new ArgumentNullException(nameof(other));
			} else if (Hash != other.Hash) {
				throw new ArgumentException($"Cannot merge {nameof(TorrentInfo)} :: {nameof(Hash)} must match", nameof(other));
			}

			Url = other.Url;
			Hash = other.Hash;
			Quality = other.Quality;
			Type = other.Type;
			Size = other.Size;
			SizeBytes = other.SizeBytes;
			DateUploaded = other.DateUploaded;
			DateUploadedUnix = other.DateUploadedUnix;
			Current = other.Current;
		}
	}
}
