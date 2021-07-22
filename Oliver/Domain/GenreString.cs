using System;
using System.Collections.Generic;

namespace Oliver.Domain {
	public class GenreString : Entity, IEquatable<GenreString> {
		public string Name { get; set; }

		// Link Properties

		public Movie Movie { get; set; }

		// Constructors

		public GenreString() : base() { }

		public GenreString(string name, Movie movie) {
			Name = name;
			Movie = movie;
		}

		// Equality

		public override bool Equals(object obj) {
			return Equals(obj as GenreString);
		}

		public bool Equals(GenreString other) {
			return other != null &&
				   Name == other.Name;
		}

		public override int GetHashCode() {
			return HashCode.Combine(Name);
		}

		public static bool operator ==(GenreString left, GenreString right) {
			return EqualityComparer<GenreString>.Default.Equals(left, right);
		}

		public static bool operator !=(GenreString left, GenreString right) {
			return !(left == right);
		}
	}
}
