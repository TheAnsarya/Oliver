using System.Collections.Generic;

namespace Oliver.Domain.Config {
	public class YtsType {
		public string Name { get; set; }

		public string Regex { get; set; }

		public List<FolderPair> Folders { get; set; }
	}
}
