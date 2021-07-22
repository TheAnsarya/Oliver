using System.Collections.Generic;
using Oliver.Constants;

namespace Oliver.Domain {
	public class TorrentFile : Entity {
		public string Hash { get; set; }

		public string Filename { get; set; }

		public string FilePath { get; set; }

		public byte[] Content { get; set; }

		public int PieceSize { get; set; }

		public byte[] Pieces { get; set; }

		public bool IsHashed => MD5 != null;

		public string MD5 { get; set; }

		public string SHA1 { get; set; }

		public string SHA256 { get; set; }

		public bool IsMultiFile { get; set; }

		public TorrentAnalyzedStatus AnalyzedStatus { get; set; }

		public bool IsVerified { get; set; }

		// Link Properties

		public virtual TorrentInfo Info { get; set; }

		public virtual List<TorrentDataFile> TorrentDataFiles { get; set; }
	}
}
