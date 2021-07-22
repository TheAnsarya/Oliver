using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BencodeNET.Parsing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oliver.Constants;
using Oliver.Data;
using Oliver.Exceptions;
using Oliver.Services.Interfaces;

namespace Oliver.Services {
	public class CleanupService : ICleanupService {
		private const int BatchSize = 100;

		private readonly ILogger Logger;

		private readonly OliverContext Context;

		private readonly IConfiguration _config;

		private readonly IHttpClientFactory ClientFactory;

		public CleanupService(ILogger logger, OliverContext context, IConfiguration config, IHttpClientFactory clientFactory) {
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			Context = context ?? throw new ArgumentNullException(nameof(context));
			_config = config;
			ClientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
		}

		public async Task AddFoldersAndExtras() {
			var folders = _config.GetValue<string[]>("Folders:VideoOnly");
			var extras = _config["Folders:Extras"];

			var skip = 0;
			var errors = 0;
			var baseQuery =
				Context.TorrentFiles
					.Where(x =>
						!x.IsVerified
						&& x.AnalyzedStatus == TorrentAnalyzedStatus.Analyzed
						&& (x.IsMultiFile == true)
						&& x.TorrentDataFiles.Any(y => y.Filename == null));
			var total = baseQuery.Count();

			var parser = new BencodeParser();

			while (skip < total) {
				var torrentFiles =
					baseQuery
						.Include("DataFiles")
						.Skip(skip)
						.Take(BatchSize)
						.ToList();

				foreach (var torrentFile in torrentFiles) {
					try {
						//	var torrent = parser.Parse<Torrent>(torrentFile.Content);

						//	if (torrent.FileMode != TorrentFileMode.Multi) {
						//		throw new Exception($"Torrent is not in multi file mode ({nameof(AddFoldersAndExtras)})");
						//	}

						//	var movieFile = torrent.Files.OrderByDescending(x => x.FileSize).First();

						var movieFile = torrentFile.TorrentDataFiles.OrderByDescending(x => x.Size).First();
						var otherFiles = torrentFile.TorrentDataFiles.OrderByDescending(x => x.Size).Skip(1);

						foreach (var folder in folders) {
							var movieFilePath = Path.Combine(folder, movieFile.Filename);
							if (!File.Exists(movieFilePath)) {
								continue;
							}

							// Make folders for files
							// TODO: combined is temporary, make better folder management
							var basePath = $"{Path.GetDirectoryName(folder)}-combined";
							foreach (var file in torrentFile.TorrentDataFiles) {
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
						Logger.LogError(ex, $"Cannot verify torrent {torrentFile.Hash}");

						if (++errors > 100) {
							var msg = $"Too many errors, aborting command {nameof(AddFoldersAndExtras)}";
							Logger.LogError(msg);
							throw new TooManyErrorsException(msg);
						}
					}
				}

				skip += BatchSize;
			}
		}
	}
}
