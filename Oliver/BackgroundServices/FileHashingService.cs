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
	public class FileHashingService : BackgroundService {
		private ILogger<FileHashingService> Logger { get; }
		private OliverContext Context { get; }
		private IHashService Hasher { get; }
		private ServicesOptions Settings { get; }

		public FileHashingService(ILogger<FileHashingService> logger, OliverContext context, IHashService hasher, IOptions<ServicesOptions> settings) {
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			Context = context ?? throw new ArgumentNullException(nameof(context));
			Hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
			Settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
			Logger.LogDebug($"{nameof(FileHashingService)} is starting.");

			_ = stoppingToken.Register(() => Logger.LogDebug($"{nameof(FileHashingService)} background task is stopping."));

			while (!stoppingToken.IsCancellationRequested) {
				Logger.LogDebug($"{nameof(FileHashingService)} task doing background work.");

				await HashFiles();

				// Delay processing only if nothing is left to process
				// TODO: if a file never hashes correctly this could loop infinitely trying to do so again and again
				// TODO: this isn't likely unless the file doesn't exist but we're not handling that yet
				if (HashesLeft > 0) {
					await Task.Yield();
				} else {
					await Task.Delay(Settings.CheckUpdateTime, stoppingToken);
				}
			}

			Logger.LogDebug($"{nameof(FileHashingService)} background task is stopping.");
		}

		private int HashesLeft => Context.DataFiles.Where(x => x.MD5 == null).Count();

		// TODO: handle errors
		private async Task HashFiles() {
			var dataFiles = Context.DataFiles
				.Where(x => x.MD5 == null)
				.OrderBy(x => x.LastHashAttempt)
				.Take(Settings.HashFilesBatchSize)
				.ToList();

			foreach (var item in dataFiles) {
				item.LastHashAttempt = DateTime.Now;
				await Context.SaveChangesAsync();

				// TODO: handle when file is missing
				if (File.Exists(item.FilePath)) {
					using var stream = File.OpenRead(item.FilePath);

					var hashes = await Hasher.GetAll(stream);

					item.MD5 = hashes.MD5;
					item.SHA1 = hashes.SHA1;
					item.SHA256 = hashes.SHA256;

					await Context.SaveChangesAsync();
				}
			}
		}
	}
}
