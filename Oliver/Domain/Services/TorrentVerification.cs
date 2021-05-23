using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oliver.Constants;

namespace Oliver.Domain.Services {
	public class TorrentVerification {
		public bool IsVerified => Status == TorrentVerificationStatus.Verified;

		// Why it might not be verified:
		public TorrentVerificationStatus Status { get; set; }

		public Exception StatusException { get; set; }

		public List<DataFile> BadDataFiles { get; set; }

		public bool AllPiecesMatched { get => Pieces.All(x => x); }

		public bool FilenamesMatch { get => Filenames.All(x => x); }

		public bool HasUnknownFiles { get => UnknownFiles.Count() > 0; }

		// Files in order indicating if the files exist
		public bool[] Filenames { get; set; }

		// Relative paths of files in the folder that are not in the torrent
		public string[] UnknownFiles { get; set; }

		// Files in order indicating if all the pieces in the file have matching hashes
		public bool[] Files { get; set; }

		// Pieces in order indicating if the SHA1 hashes match
		public bool[] Pieces { get; set; }
	}
}
