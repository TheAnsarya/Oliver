using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Oliver.Exceptions {
	public class ExceptionFilter : IExceptionFilter {
		public void OnException(ExceptionContext context) {
			var message = "An error occurred while attempting to satisfy the request.";
			var statusCode = (int)HttpStatusCode.InternalServerError;

			if (context == null) {
				throw new ArgumentNullException(nameof(context));
			}

			switch (context.Exception) {
				case InvalidInputException:
					message = context.Exception.Message;
					statusCode = (int)HttpStatusCode.BadRequest;
					break;
				case NotFoundException:
					message = context.Exception.Message;
					statusCode = (int)HttpStatusCode.NotFound;
					break;
			}

			context.Result = new ContentResult() {
				Content = message,
				StatusCode = statusCode
			};

			context.ExceptionHandled = true;
		}
	}
}
