namespace Oliver.Domain;

public class TorrentTracker : Entity {
	public string Url { get; set; } = string.Empty;

	public int Tier { get; set; }

	// Navigation
	public Guid TorrentInfoId { get; set; }

	public TorrentInfo? TorrentInfo { get; set; }
}
