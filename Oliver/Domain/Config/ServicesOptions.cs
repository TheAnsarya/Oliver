namespace Oliver.Domain.Config {
	public class ServicesOptions {
		public const string SectionName = "Services";

		public int CheckUpdateTime { get; set; }

		public int YtsUpdateDelay { get; set; }

		public int FindFilesBatchSize { get; set; }

		public int HashFilesBatchSize { get; set; }
	}
}
