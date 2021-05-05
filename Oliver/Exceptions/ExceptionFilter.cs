using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Oliver.Exceptions {
	public class ExceptionFilter : IExceptionFilter {
		public void OnException(ExceptionContext filterContext) {
			var message = "An error occurred while attempting to satisfy the request.";
			var statusCode = (int)HttpStatusCode.InternalServerError;

			if (filterContext.Exception is NotFoundException) {
				message = filterContext.Exception.Message;
				statusCode = (int)HttpStatusCode.NotFound;
			} else if (filterContext.Exception is InvalidInputException) {
				message = filterContext.Exception.Message;
				statusCode = (int)HttpStatusCode.BadRequest;
			}

			filterContext.Result = new ContentResult() {
				Content = message,
				StatusCode = statusCode
			};

			filterContext.ExceptionHandled = true;
		}
	}
}
