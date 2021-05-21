using System.Collections.Generic;

namespace Oliver.Domain.Config {
	public class FoldersOptions {
		public const string SectionName = "Folders";

		public string Extras { get; set; }

		public List<string> Complete { get; set; }

		public List<string> VideoOnly { get; set; }

		public Torrents Torrents { get; set; }
	}
}
