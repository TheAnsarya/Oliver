using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oliver.Data;
using Oliver.Extensions;
using Oliver.Domain;
using Oliver.Domain.YTS.Requests;
using Oliver.Domain.YTS.Responses;
using Oliver.Services.Interfaces;

namespace Oliver.Services {
	public class YtsService : IYtsService {
		private const int BATCH_TORRENTS_SIZE = 100;

		private readonly IConfiguration _config;

		private readonly IHttpClientFactory _clientFactory;

		private readonly OliverContext _context;

		private readonly ILogger _logger;

		private readonly IHashService _hasher;

		public YtsService(IConfiguration config, IHttpClientFactory clientFactory, OliverContext context, ILogger logger, IHashService hasher) {
			_config = config;
			_clientFactory = clientFactory;
			_context = context;
			_logger = logger;
			_hasher = hasher;
		}

		public async Task<Response<ListMoviesData>> FetchMoviesList(ListMoviesRequest request = null) {
			if (request == null) {
				request = new ListMoviesRequest();
			}

			var baseAddress = _config["Yts:BaseAddress"];
			var endpoint = $"{baseAddress}api/v2/list_movies.json";
			var url = $"{endpoint}{request.QueryString()}";

			var client = _clientFactory.CreateClient();
			var response = await client.GetStringAsync(url);

			var moviesList = JsonSerializer.Deserialize<Response<ListMoviesData>>(response);

			return moviesList;
		}

		public async Task<Movie> AddOrUpdateMovie(YtsMovie dto) {
			if (dto == null) {
				throw new ArgumentNullException(nameof(dto));
			}

			var movie = _context.Movies.Where(x => x.YtsId == dto.Id).FirstOrDefault();

			if (movie == null) {
				movie = new Movie(dto);
				_context.Add(movie);
			} else {
				var other = new Movie(dto);
				if (movie == other) {
					return movie;
				}

				movie.Update(other);
			}

			await _context.SaveChangesAsync();
			return movie;
		}

		public async Task FetchMissingTorrents() {
			var folder = _config["Folders:Torrents:Current"];
			var oldFolder = _config["Folders:Torrents:Old"];

			// TODO: Move to a startup task
			if (!Directory.Exists(folder)) {
				Directory.CreateDirectory(folder);
			}
			if (!Directory.Exists(oldFolder)) {
				Directory.CreateDirectory(oldFolder);
			}

			var client = _clientFactory.CreateClient();

			var skip = 0;
			var errors = 0;
			var total =
				_context.TorrentInfos
					.Where(x => x.TorrentFile == null)
					.Count();

			while (skip < total) {
				var infos =
					_context.TorrentInfos
						.Where(x => x.TorrentFile == null)
						.Skip(skip)
						.Take(BATCH_TORRENTS_SIZE)
						.ToList();

				foreach (var info in infos) {
					try {
						var data = await client.GetByteArrayAsync(info.Url);
						var path = Path.Combine(folder, $"{info.Hash}.torrent");

						if (File.Exists(path)) {
							var oldData = await File.ReadAllBytesAsync(path);
							if (!data.IsSame(oldData)) {
								var oldPath = Path.Combine(oldFolder, $"{info.Hash}-{DateTime.Now.Timestamp()}.torrent");
								File.Move(path, oldPath);
								await File.WriteAllBytesAsync(path, data);
							}
						} else {
							await File.WriteAllBytesAsync(path, data);
						}

						var hashes = await _hasher.GetAll(data);

						var torrentFile = new TorrentFile() {
							Content = data,
							Filename = Path.GetFileName(path),
							FilePath = path,
							Hash = info.Hash,
							Info = info,
							MD5 = hashes.MD5,
							SHA1 = hashes.SHA1,
							SHA256 = hashes.SHA256
						};

						info.TorrentFile = torrentFile;

						_context.Add(torrentFile);
						await _context.SaveChangesAsync();
					} catch (Exception ex) {
						_logger.LogError(ex, $"Cannot download torrent {info.Url}");
						
						if (++errors > 100) {
							var msg = $"Too many errors, aborting command {nameof(FetchMissingTorrents)}";
							_logger.LogError(msg);
							throw new Exception(msg);
						}

					}
				}

				skip += BATCH_TORRENTS_SIZE;
			}
		}
	}
}
