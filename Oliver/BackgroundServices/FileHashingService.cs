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
		private readonly ILogger<FileHashingService> _logger;
		private readonly OliverContext _context;
		private readonly IHashService _hasher;
		private readonly ServicesOptions _settings;

		public FileHashingService(ILogger<FileHashingService> logger, OliverContext context, IHashService hasher, IOptions<ServicesOptions> settings) {
			_logger = logger;
			_context = context;
			_hasher = hasher;
			_settings = settings.Value;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
			_logger.LogDebug($"{nameof(FileHashingService)} is starting.");

			stoppingToken.Register(() => _logger.LogDebug($"{nameof(FileHashingService)} background task is stopping."));

			while (!stoppingToken.IsCancellationRequested) {
				_logger.LogDebug($"{nameof(FileHashingService)} task doing background work.");

				await HashFiles();

				// Delay processing only if nothing is left to process
				// TODO: if a file never hashes correctly this could loop infinitely trying to do so again and again
				// TODO: this isn't likely unless the file doesn't exist but we're not handling that yet
				if (HashesLeft > 0) {
					await Task.Yield();
				} else {
					await Task.Delay(_settings.CheckUpdateTime, stoppingToken);
				}
			}

			_logger.LogDebug($"{nameof(FileHashingService)} background task is stopping.");
		}

		private int HashesLeft => _context.DataFiles.Where(x => x.MD5 == null).Count();

		// TODO: handle errors
		private async Task HashFiles() {
			var dataFiles = _context.DataFiles
				.Where(x => x.MD5 == null)
				.OrderBy(x => x.LastHashAttempt)
				.Take(_settings.HashFilesBatchSize)
				.ToList();

			foreach (var item in dataFiles) {
				item.LastHashAttempt = DateTime.Now;
				await _context.SaveChangesAsync();

				// TODO: handle when file is missing
				var stream = File.OpenRead(item.FilePath);

				var hashes = await _hasher.GetAll(stream);

				item.MD5 = hashes.MD5;
				item.SHA1 = hashes.SHA1;
				item.SHA256 = hashes.SHA256;

				await _context.SaveChangesAsync();
			}
		}
	}
}
