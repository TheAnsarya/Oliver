using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BencodeNET.Parsing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oliver.Data;
using Oliver.Services.Interfaces;

namespace Oliver.Services {
	public class CleanupService : ICleanupService {
		private const int BATCH_SIZE = 100;

		private readonly IConfiguration _config;

		private readonly ILogger _logger;

		private readonly IHttpClientFactory _clientFactory;

		private readonly OliverContext _context;

		public CleanupService(IConfiguration config, ILogger logger, IHttpClientFactory clientFactory, OliverContext context) {
			_config = config;
			_logger = logger;
			_clientFactory = clientFactory;
			_context = context;
		}

		public async Task AddFoldersAndExtras() {
			var folders = _config.GetValue<string[]>("Folders:VideoOnly");
			var extras = _config["Folders:Extras"];

			var skip = 0;
			var errors = 0;
			var baseQuery =
				_context.TorrentFiles
						.Where(x =>
							!x.Verified
							&& x.Analyzed
							&& (x.MultiFile == true)
							&& x.DataFiles.Any(y => y.Filename == null));
			var total = baseQuery.Count();

			var parser = new BencodeParser();

			while (skip < total) {
				var torrentFiles =
					baseQuery
						.Include("DataFiles")
						.Skip(skip)
						.Take(BATCH_SIZE)
						.ToList();

				foreach (var torrentFile in torrentFiles) {
					try {
						//	var torrent = parser.Parse<Torrent>(torrentFile.Content);

						//	if (torrent.FileMode != TorrentFileMode.Multi) {
						//		throw new Exception($"Torrent is not in multi file mode ({nameof(AddFoldersAndExtras)})");
						//	}

						//	var movieFile = torrent.Files.OrderByDescending(x => x.FileSize).First();

						var movieFile = torrentFile.DataFiles.OrderByDescending(x => x.Size).First();
						var otherFiles = torrentFile.DataFiles.OrderByDescending(x => x.Size).Skip(1);

						foreach (var folder in folders) {
							var movieFilePath = Path.Combine(folder, movieFile.Filename);
							if (!File.Exists(movieFilePath)) {
								continue;
							}

							// Make folders for files
							// TODO: combined is temporary, make better folder management
							var basePath = $"{Path.GetDirectoryName(folder)}-combined";
							foreach (var file in torrentFile.DataFiles) {
								var tempPath = Path.Combine(basePath, file.PartialFolder);
								if (!Directory.Exists(tempPath)) {
									Directory.CreateDirectory(tempPath);
								}
							}

							// Move movie file
							var newMoviePath = Path.Combine(basePath, movieFile.PartialPath);
							if (File.Exists(newMoviePath)) {
								// TODO: compare and do something if inequal
							} else {
								File.Move(movieFilePath, newMoviePath);
							}
						}




					} catch (Exception ex) {
						_logger.LogError(ex, $"Cannot verify torrent {torrentFile.Hash}");

						if (++errors > 100) {
							var msg = $"Too many errors, aborting command {nameof(AddFoldersAndExtras)}";
							_logger.LogError(msg);
							throw new Exception(msg);
						}

					}
				}

				skip += BATCH_SIZE;
			}
		}
	}
}
