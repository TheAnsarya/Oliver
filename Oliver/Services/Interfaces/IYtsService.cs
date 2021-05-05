using System.Threading.Tasks;
using Oliver.Domain.YTS.Requests;
using Oliver.Domain.YTS.Responses;

namespace Oliver.Services.Interfaces {
	public interface IYtsService {
		Task<Response<ListMoviesData>> FetchMoviesList(ListMoviesRequest request = null);
	}
}
