using System;

namespace Oliver.Exceptions {
	public class InvalidInputException : Exception {
		public InvalidInputException() : base() { }

		public InvalidInputException(string message) : base(message) { }

		public InvalidInputException(string message, Exception innerException) : base(message, innerException) { }
	}
}
