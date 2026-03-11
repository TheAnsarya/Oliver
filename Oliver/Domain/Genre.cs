namespace Oliver.Domain;

public class Genre : Entity {
	public string Name { get; set; } = string.Empty;

	// Navigation
	public Guid MovieId { get; set; }

	public Movie? Movie { get; set; }
}
