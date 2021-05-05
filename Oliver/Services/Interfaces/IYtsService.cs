using Oliver.Domain.YTS.Requests;
using Oliver.Domain.YTS.Responses;
using System.Threading.Tasks;

namespace Oliver.Services.Interfaces
{
	public interface IYtsService
	{
		Task<Response<ListMoviesData>> FetchMoviesList(ListMoviesRequest request = null);
	}
}
