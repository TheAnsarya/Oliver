using System;
using System.IO;

namespace Oliver.Domain {
	public class DataFile : Entity {
		// Example: @"i:\yts-full.mx\"
		public string BasePath { get; set; }

		// Full file path
		// Example #1: @"i:\yts-full.mx\The Olympic Champ (1942) [1080p] [WEBRip] [YTS.MX]\The.Olympic.Champ.1942.1080p.WEBRip.x264.AAC-[YTS.MX].mp4"
		// Example #2: @"i:\yts-full.mx\The Olympic Champ (1942) [1080p] [WEBRip] [YTS.MX]\Subs\Norsk.nor.srt"
		public string FilePath { get; set; }

		public long Size { get; set; }

		public string MD5 { get; set; }

		public string SHA1 { get; set; }

		public string SHA256 { get; set; }

		public DateTime LastHashAttempt { get; set; }

		public DateTime LastVerifiedAttempt { get; set; }

		public bool Verified { get; set; }

		// Link Properties

		public virtual TorrentDataFile TorrentDataFile { get; set; }
	}
}
