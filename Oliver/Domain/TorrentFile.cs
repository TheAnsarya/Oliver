namespace Oliver.Domain {
	public class TorrentFile : Entity {
		public TorrentInfo Info { get; set; }

		public string Filename { get; set; }

		public string FilePath { get; set; }

		public byte[] Content { get; set; }
	}
}
