namespace Oliver.Domain;

public class TorrentFileEntry : Entity {
	public string FilePath { get; set; } = string.Empty;

	public long FileSize { get; set; }

	// Navigation
	public Guid TorrentInfoId { get; set; }

	public TorrentInfo? TorrentInfo { get; set; }
}
