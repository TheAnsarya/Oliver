using System;

namespace Oliver.Exceptions {
	public class TooManyErrorsException : Exception {
		public TooManyErrorsException() : base() { }

		public TooManyErrorsException(string message) : base(message) { }

		public TooManyErrorsException(string message, Exception innerException) : base(message, innerException) { }
	}
}
