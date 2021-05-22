using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oliver.Data;
using Oliver.Domain.Config;
using Oliver.Services.Interfaces;

namespace Oliver.BackgroundServices {
	public class YtsUpdateService : BackgroundService {
		private readonly ILogger<YtsUpdateService> _logger;
		private readonly OliverContext _context;
		private readonly IYtsService _ytsService;
		private readonly ServicesOptions _settings;

		public YtsUpdateService(ILogger<YtsUpdateService> logger, OliverContext context, IYtsService ytsService, IOptions<ServicesOptions> settings) {
			_logger = logger;
			_context = context;
			_ytsService = ytsService;
			_settings = settings.Value;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
			_logger.LogDebug($"{nameof(YtsUpdateService)} is starting.");

			stoppingToken.Register(() => _logger.LogDebug($"{nameof(YtsUpdateService)} background task is stopping."));

			while (!stoppingToken.IsCancellationRequested) {
				_logger.LogDebug($"{nameof(YtsUpdateService)} task doing background work.");

				await UpdateYts();

				await Task.Delay(_settings.YtsUpdateDelay, stoppingToken);
			}

			_logger.LogDebug($"{nameof(YtsUpdateService)} background task is stopping.");
		}

		private async Task UpdateYts() {

		}
	}
}
