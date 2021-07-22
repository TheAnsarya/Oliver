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
		private ILogger<FileProcessingService> Logger { get; }
		private OliverContext Context { get; }
		private IHashService Hasher { get; }
		private ServicesOptions Settings { get; }
		private FoldersOptions FolderSettings { get; }

		public FileProcessingService(ILogger<FileProcessingService> logger, OliverContext context, IHashService hasher, IOptions<ServicesOptions> settings, IOptions<FoldersOptions> folderSettings) {
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			Context = context ?? throw new ArgumentNullException(nameof(context));
			Hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
			Settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
			FolderSettings = folderSettings?.Value ?? throw new ArgumentNullException(nameof(folderSettings));
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
			Logger.LogDebug($"{nameof(FileProcessingService)} is starting.");

			stoppingToken.Register(() =>
				Logger.LogDebug($"{nameof(FileProcessingService)} background task is stopping."));

			while (!stoppingToken.IsCancellationRequested) {
				Logger.LogDebug($"{nameof(FileProcessingService)} task doing background work.");

				//

				await Task.Delay(Settings.CheckUpdateTime, stoppingToken);
			}

			Logger.LogDebug($"{nameof(FileProcessingService)} background task is stopping.");
		}

		// TODO: Switch to matching to datafiles?
		private async Task FindFiles() {
			var dataFiles = Context.TorrentDataFiles
				.Where(x => x.DataFile == null)
				.OrderBy(x => x.LastFindAttempt)
				.Take(Settings.FindFilesBatchSize)
				.ToList();

			foreach (var item in dataFiles) {
				item.LastFindAttempt = DateTime.Now;
				foreach (var basefolder in FolderSettings.Complete) {
					var path = Path.Combine(basefolder, item.PartialPath);
					var file = new FileInfo(path);
					if (file.Exists && (file.Length == item.Size)) {

					}
				}

				await Context.SaveChangesAsync();
			}
		}
	}
}
