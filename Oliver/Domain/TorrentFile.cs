﻿using System.ComponentModel.DataAnnotations;

namespace Oliver.Domain {
	public class TorrentFile : Entity {
		public TorrentInfo Info { get; set; }

		public string Hash { get; set; }

		public string Filename { get; set; }

		public string FilePath { get; set; }

		public byte[] Content { get; set; }

		public string MD5 { get; set; }

		public string SHA1 { get; set; }

		public string SHA2 { get; set; }
	}
}
