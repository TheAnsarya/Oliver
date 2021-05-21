namespace Oliver.Domain.Config {
	public class YtsOptions {
		public const string SectionName = "Yts";

		public string BaseAddress { get; set; }

		public YtsTypes Types { get; set; }
	}
}
