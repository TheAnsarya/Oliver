using System.Collections.Generic;

namespace Oliver.Domain.Config {
	public class Folders {
		public string Extras { get; set; }

		public List<string> Complete { get; set; }

		public List<string> VideoOnly { get; set; }

		public Torrents Torrents { get; set; }
	}
}
