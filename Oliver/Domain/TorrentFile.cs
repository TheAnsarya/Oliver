using System.Collections.Generic;

namespace Oliver.Domain {
	public class TorrentFile : Entity {
		public string Hash { get; set; }

		public string Filename { get; set; }

		public string FilePath { get; set; }

		public byte[] Content { get; set; }

		public string MD5 { get; set; }

		public string SHA1 { get; set; }

		public string SHA256 { get; set; }

		public bool MultiFile { get; set; }

		public bool Analyzed { get; set; }

		public bool Verified { get; set; }

		// Link Properties

		public virtual TorrentInfo Info { get; set; }

		public virtual List<DataFile> DataFiles { get; set; }
	}
}
