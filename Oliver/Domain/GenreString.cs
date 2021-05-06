namespace Oliver.Domain {
	public class GenreString : Entity {
		public string Name { get; set; }

		public Movie Movie { get; set; }

		public GenreString() : base() { }

		public GenreString(string name, Movie movie) {
			Name = name;
			Movie = movie;
		}
	}
}
