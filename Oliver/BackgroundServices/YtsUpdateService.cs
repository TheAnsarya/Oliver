using System;
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
		private ILogger<YtsUpdateService> Logger { get; }
		private OliverContext Context { get; }
		private IYtsService YtsService { get; }
		private ServicesOptions Settings { get; }

		public YtsUpdateService(ILogger<YtsUpdateService> logger, OliverContext context, IYtsService ytsService, IOptions<ServicesOptions> settings) {
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			Context = context ?? throw new ArgumentNullException(nameof(context));
			YtsService = ytsService ?? throw new ArgumentNullException(nameof(ytsService));
			Settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
			Logger.LogDebug($"{nameof(YtsUpdateService)} is starting.");

			stoppingToken.Register(() => Logger.LogDebug($"{nameof(YtsUpdateService)} background task is stopping."));

			while (!stoppingToken.IsCancellationRequested) {
				Logger.LogDebug($"{nameof(YtsUpdateService)} task doing background work.");

				await UpdateYts();

				await Task.Delay(Settings.YtsUpdateDelay, stoppingToken);
			}

			Logger.LogDebug($"{nameof(YtsUpdateService)} background task is stopping.");
		}

		private async Task UpdateYts() {

		}
	}
}
