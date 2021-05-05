using System;

namespace Oliver.Exceptions
{
	public class InvalidInputException : Exception
	{
		public InvalidInputException(string message) : base(message)
		{
		}
	}
}
