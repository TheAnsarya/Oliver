namespace Oliver.Domain.Torrents {
	public class FileMatch {
		public string FileName { get; set; }

		public string FileNameFull { get; set; }

		public long ExpectedSize { get; set; }

		public long Size { get; set; }

		public bool Exists { get; set; }

		public int FirstPiece { get; set; }

		public int LastPiece { get; set; }
	}
}
