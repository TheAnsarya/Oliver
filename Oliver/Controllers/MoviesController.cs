using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Oliver.Controllers {
	[ApiController]
	[Route("[controller]")]
	public class MoviesController : ControllerBase {
		private ILogger<MoviesController> Logger { get; }

		public MoviesController(ILogger<MoviesController> logger) => Logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}
}
