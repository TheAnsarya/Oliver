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
	public class FileProcessingService : BackgroundService {
		private readonly ILogger<FileProcessingService> _logger;
		private readonly ServicesOptions _settings;
		private readonly FoldersOptions _folderSettings;
		private readonly IHashService _hasher;
		private readonly OliverContext _context;

		public FileProcessingService(ILogger<FileProcessingService> logger, IHashService hasher, OliverContext context, IOptions<ServicesOptions> settings, IOptions<FoldersOptions> folderSettings) {
			_logger = logger;
			_hasher = hasher;
			_context = context;
			_settings = settings.Value;
			_folderSettings = folderSettings.Value;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
			_logger.LogDebug($"{nameof(FileProcessingService)} is starting.");

			stoppingToken.Register(() =>
				_logger.LogDebug($"{nameof(FileProcessingService)} background task is stopping."));

			while (!stoppingToken.IsCancellationRequested) {
				_logger.LogDebug($"{nameof(FileProcessingService)} task doing background work.");

				//

				await Task.Delay(_settings.CheckUpdateTime, stoppingToken);
			}

			_logger.LogDebug($"{nameof(FileProcessingService)} background task is stopping.");
		}

		// TODO: Switch to matching to datafiles?
		private async Task FindFiles() {
			var dataFiles = _context.TorrentDataFiles
				.Where(x => x.DataFile == null)
				.OrderBy(x => x.LastFindAttempt)
				.Take(_settings.FindFilesBatchSize)
				.ToList();

			foreach (var item in dataFiles) {
				item.LastFindAttempt = DateTime.Now;
				foreach (var basefolder in _folderSettings.Complete) {
					var path = Path.Combine(basefolder, item.PartialPath);
					var file = new FileInfo(path);
					if (file.Exists && (file.Length == item.Size)) {

					}
				}

				await _context.SaveChangesAsync();
			}
		}
	}
}
