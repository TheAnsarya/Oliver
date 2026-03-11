namespace Oliver.Domain;

public class Movie : Entity {
	public int YtsId { get; set; }

	public string? Url { get; set; }

	public string? ImdbCode { get; set; }

	public string Title { get; set; } = string.Empty;

	public string? TitleEnglish { get; set; }

	public string? TitleLong { get; set; }

	public string? Slug { get; set; }

	public int Year { get; set; }

	public decimal Rating { get; set; }

	public int Runtime { get; set; }

	public string? Summary { get; set; }

	public string? DescriptionFull { get; set; }

	public string? Synopsis { get; set; }

	public string? YtTrailerCode { get; set; }

	public string? Language { get; set; }

	public string? MpaRating { get; set; }

	public string? BackgroundImage { get; set; }

	public string? BackgroundImageOriginal { get; set; }

	public string? SmallCoverImage { get; set; }

	public string? MediumCoverImage { get; set; }

	public string? LargeCoverImage { get; set; }

	public string? State { get; set; }

	public string? DateUploaded { get; set; }

	public long DateUploadedUnix { get; set; }

	// Navigation properties
	public List<TorrentInfo> Torrents { get; set; } = [];

	public List<Genre> Genres { get; set; } = [];

	// Download tracking
	public bool ImagesDownloaded { get; set; }

	public DateTime? LastSyncedAt { get; set; }
}
