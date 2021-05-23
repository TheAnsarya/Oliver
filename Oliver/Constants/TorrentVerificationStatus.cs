using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oliver.Constants {
	public enum TorrentVerificationStatus {
		NotAnalyzed,
		MissingDataFiles,
		TorrentCannotBeParsed,
		DataFilesNotFound,

		Verified
	}
}
