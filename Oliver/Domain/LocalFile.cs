namespace Oliver.Domain;

public class LocalFile : Entity {
	public string FilePath { get; set; } = string.Empty;

	public string FileName { get; set; } = string.Empty;

	public string Extension { get; set; } = string.Empty;

	public long FileSize { get; set; }

	public DateTime FileModified { get; set; }

	// Matching
	public Guid? MatchedMovieId { get; set; }

	public Movie? MatchedMovie { get; set; }

	public Guid? MatchedTorrentId { get; set; }

	public TorrentInfo? MatchedTorrent { get; set; }
}
