namespace Oliver.Domain;

public class TorrentInfo : Entity {
	public string? Url { get; set; }

	public string Hash { get; set; } = string.Empty;

	public string? Quality { get; set; }

	public string? Type { get; set; }

	public string? Size { get; set; }

	public long SizeBytes { get; set; }

	public int Seeds { get; set; }

	public int Peers { get; set; }

	public string? DateUploaded { get; set; }

	public long DateUploadedUnix { get; set; }

	// Download tracking
	public bool TorrentFileDownloaded { get; set; }

	public string? TorrentFilePath { get; set; }

	// Navigation
	public Guid MovieId { get; set; }

	public Movie? Movie { get; set; }
}
