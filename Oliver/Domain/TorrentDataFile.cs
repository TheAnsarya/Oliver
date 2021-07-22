using System;
using System.IO;

namespace Oliver.Domain {
	public class TorrentDataFile : Entity {
		// Example: @"The Olympic Champ (1942) [1080p] [WEBRip] [YTS.MX]"
		public string Folder { get; set; }

		// Example #1: ""
		// Example #2: @"Subs"
		public string SubPath { get; set; }

		// Just the filename
		// Example #1: @"The.Olympic.Champ.1942.1080p.WEBRip.x264.AAC-[YTS.MX].mp4"
		// Example #2: @"Norsk.nor.srt"
		public string Filename { get; set; }

		// Example #1: @"The Olympic Champ (1942) [1080p] [WEBRip] [YTS.MX]"
		// Example #2: @"The Olympic Champ (1942) [1080p] [WEBRip] [YTS.MX]\Subs"
		public string PartialFolder { get => Path.Combine(Folder, SubPath); }

		// Example #1: @"The Olympic Champ (1942) [1080p] [WEBRip] [YTS.MX]\The.Olympic.Champ.1942.1080p.WEBRip.x264.AAC-[YTS.MX].mp4"
		// Example #2: @"The Olympic Champ (1942) [1080p] [WEBRip] [YTS.MX]\Subs\Norsk.nor.srt"
		public string PartialPath { get => Path.Combine(Folder, SubPath, Filename); }

		public long Size { get; set; }

		public DateTime LastFindAttempt { get; set; }

		public DateTime LastVerifiedAttempt { get; set; }

		public bool Verified { get; set; }

		// Link Properties

		public virtual TorrentFile TorrentFile { get; set; }

		public virtual DataFile DataFile { get; set; }
	}
}
