using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BencodeNET.Parsing;
using BencodeNET.Torrents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oliver.Constants;
using Oliver.Data;
using Oliver.Domain;
using Oliver.Domain.YTS.Requests;
using Oliver.Domain.YTS.Responses;
using Oliver.Extensions;
using Oliver.Services.Interfaces;

namespace Oliver.Services {
	public class YtsService : IYtsService {
		private const int BATCH_TORRENTS_SIZE = 100;

		private ILogger Logger { get; }

		private OliverContext Context { get; }

		private IConfiguration Config { get; }

		private IHttpClientFactory ClientFactory { get; }

		private IHashService Hasher { get; }

		public YtsService(IConfiguration config, ILogger logger, IHttpClientFactory clientFactory, OliverContext context, IHashService hasher) {
			Config = config;
			Logger = logger;
			ClientFactory = clientFactory;
			Context = context;
			Hasher = hasher;
		}

		public async Task<Response<ListMoviesData>> FetchMoviesList(ListMoviesRequest request = null) {
			if (request == null) {
				request = new ListMoviesRequest();
			}

			var baseAddress = Config["Yts:BaseAddress"];
			var endpoint = $"{baseAddress}api/v2/list_movies.json";
			var url = $"{endpoint}{request.QueryString()}";

			using var client = ClientFactory.CreateClient();
			var response = await client.GetStringAsync(url);

			var moviesList = JsonSerializer.Deserialize<Response<ListMoviesData>>(response);

			return moviesList;
		}

		// The bool indicates if the movie was added/updated (true) or if it already exists and no changes were made (false)
		public async Task<(Movie, bool)> AddOrUpdateMovie(YtsMovie dto) {
			if (dto == null) {
				throw new ArgumentNullException(nameof(dto));
			}

			var movie = Context.Movies.Where(x => x.YtsId == dto.Id).FirstOrDefault();

			if (movie == null) {
				movie = new Movie(dto);
				Context.Add(movie);
			} else {
				var other = new Movie(dto);
				if (movie == other) {
					return (movie, false);
				}

				movie.Update(other);
			}

			await Context.SaveChangesAsync();
			return (movie, true);
		}

		public async Task<IList<Movie>> AddMovies(ListMoviesData movieList) {
			if (movieList == null) {
				throw new ArgumentNullException(nameof(movieList));
			}

			var updatedMovies = new List<Movie>();

			foreach (var dto in movieList.Movies) {
				var (movie, updated) = await AddOrUpdateMovie(dto);
				if (updated) {
					updatedMovies.Add(movie);
				}
			}

			return updatedMovies;
		}

		public async Task<TorrentFile> AddTorrentFile(TorrentInfo info) {
			if (info is null) {
				throw new ArgumentNullException(nameof(info));
			}

			using var client = ClientFactory.CreateClient();
			var parser = new BencodeParser();

			var folder = Config["Folders:Torrents:Current"];
			var oldFolder = Config["Folders:Torrents:Old"];

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

			var hashes = await Hasher.GetAll(data);

			var torrentFile = new TorrentFile() {
				Content = data,
				Filename = Path.GetFileName(path),
				FilePath = path,
				Hash = info.Hash,
				Info = info,
				MD5 = hashes.MD5,
				SHA1 = hashes.SHA1,
				SHA256 = hashes.SHA256,
			};

			info.TorrentFile = torrentFile;

			Context.Add(torrentFile);
			await Context.SaveChangesAsync();

			return torrentFile;
		}

		// Returns null if torrent cannot be parsed
		// TODO: already parsed, but not is not multi so datafiles weren't added?
		public async Task<IList<TorrentDataFile>> AnalyzeTorrentFile(TorrentFile torrentFile) {
			if (torrentFile is null) {
				throw new ArgumentNullException(nameof(torrentFile));
			}

			var parser = new BencodeParser();
			Torrent torrent = null;

			try {
				torrent = parser.Parse<Torrent>(torrentFile.Content);
			} catch (Exception ex) {
				Logger.LogError(ex, $"Torrent cannot be parsed {torrentFile.Hash}");
			}

			if (torrent == null) {
				torrentFile.AnalyzedStatus = TorrentAnalyzedStatus.NotParsable;

			} else {
				torrentFile.IsMultiFile = torrent.FileMode == TorrentFileMode.Multi;
				torrentFile.PieceSize = (int)torrent.PieceSize;
				torrentFile.Pieces = torrent.Pieces;

				// TODO: finish fixing this method
				if (torrent.FileMode != TorrentFileMode.Multi) {
					// TODO: possibly fix?
					throw new NotImplementedException("Torrent is not in multi file mode.");
				}

				torrentFile.TorrentDataFiles = torrent.Files.Select(x => new TorrentDataFile {
					TorrentFile = torrentFile,
					Filename = x.FileName,
					Folder = x.Path[0],
					SubPath = string.Join(Path.DirectorySeparatorChar, x.Path.Skip(1).SkipLast(1)),
					Size = x.FileSize
				}).ToList();


				torrentFile.AnalyzedStatus = TorrentAnalyzedStatus.Analyzed;

				Context.AddRange(torrentFile.TorrentDataFiles);
			}

			await Context.SaveChangesAsync();

			return torrentFile.TorrentDataFiles;
		}
	}
}
