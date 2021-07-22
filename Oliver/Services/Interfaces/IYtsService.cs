using System.Collections.Generic;
using System.Threading.Tasks;
using Oliver.Domain;
using Oliver.Domain.YTS.Requests;
using Oliver.Domain.YTS.Responses;

namespace Oliver.Services.Interfaces {
	public interface IYtsService {
		Task<Response<ListMoviesData>> FetchMoviesList(ListMoviesRequest request = null);

		// The bool indicates if the movie was added/updated (true) or if it already exists and no changes were made (false)
		Task<(Movie, bool)> AddOrUpdateMovie(YtsMovie dto);

		Task<IList<Movie>> AddMovies(ListMoviesData movieList);

		Task<TorrentFile> AddTorrentFile(TorrentInfo info);

		Task<IList<TorrentDataFile>> AnalyzeTorrentFile(TorrentFile torrentFile);
	}
}
