using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oliver.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MoviesController : ControllerBase
	{
		private readonly ILogger<MoviesController> _logger;

		public MoviesController(ILogger<MoviesController> logger)
		{
			_logger = logger;
		}
	}
}
