using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Oliver.Domain.YTS.Requests {
	// TODO: A lot of this class seems templatable.
	public class ListMoviesRequest {
		public const int DEFAULT_LIMIT = 20;
		public const int DEFAULT_PAGE = 20;
		public const string DEFAULT_QUALITY = "All";
		public const int DEFAULT_MINIMUM_RATING = 0;
		public const string DEFAULT_QUERY_TERM = "0";
		public const string DEFAULT_GENRE = "All";
		public const string DEFAULT_SORT_BY = "date_added";
		public const string DEFAULT_ORDER_BY = "desc";
		public const bool DEFAULT_WITH_RT_RATINGS = false;

		// The limit of results per page that has been set
		// between 1 - 50 (inclusive)
		// Default: 20
		// Query parameter: limit
		private int _limit = DEFAULT_LIMIT;
		public int Limit {
			get => _limit;
			set {
				if ((value < 1) || (value > 50)) {
					throw new ArgumentOutOfRangeException(nameof(value));
				}
				_limit = value;
			}
		}

		// Used to see the next page of movies, eg limit=15 and page = 2 will show you movies 15-30
		// Integer(Unsigned)
		// Default: 1
		// Query parameter: page
		private int _page = DEFAULT_PAGE;
		public int Page {
			get => _page;
			set {
				if (value < 1) {
					throw new ArgumentOutOfRangeException(nameof(value));
				}
				_page = value;
			}
		}

		// Used to filter by a given quality
		// String(720p, 1080p, 2160p, 3D) 
		// Default: "All"
		// Query parameter: quality
		private string _quality = DEFAULT_QUALITY;
		public string Quality {
			get => _quality;
			set {
				if (!(new string[] { "720p", "1080p", "2160p", "3D", "All" }).Contains(value)) {
					throw new ArgumentOutOfRangeException(nameof(value));
				}
				_quality = value;
			}
		}

		// Used to filter movie by a given minimum IMDb rating
		// Integer between 0 - 9 (inclusive)
		// Default: 0
		// Query parameter: minimum_rating
		private int _minimumRating = DEFAULT_MINIMUM_RATING;
		public int MinimumRating {
			get => _minimumRating;
			set {
				if ((value < 0) || (value > 9)) {
					throw new ArgumentOutOfRangeException(nameof(value));
				}
				_minimumRating = value;
			}
		}

		// Used for movie search, matching on: Movie Title/IMDb Code, Actor Name/IMDb Code, Director Name/IMDb Code
		// String
		// Default: "0"
		// Query parameter: query_term
		public string QueryTerm { get; set; } = DEFAULT_QUERY_TERM;

		// Used to filter by a given genre (See http://www.imdb.com/genre/ for full list)
		// String
		// Default: "All"
		// Query parameter: genre
		public string Genre { get; set; } = DEFAULT_GENRE;

		// Sorts the results by choosen value
		// String (title, year, rating, peers, seeds, download_count, like_count, date_added)
		// Default: "date_added"
		// Query parameter: sort_by
		private string _sortBy = DEFAULT_SORT_BY;
		public string SortBy {
			get => _sortBy;
			set {
				if (!(new string[] { "title", "year", "rating", "peers", "seeds", "download_count", "like_count", "date_added" }).Contains(value)) {
					throw new ArgumentOutOfRangeException(nameof(value));
				}
				_sortBy = value;
			}
		}

		// Orders the results by either Ascending or Descending order
		// String (desc, asc)
		// Default: "desc"
		// Query parameter: order_by
		private string _orderBy = DEFAULT_ORDER_BY;
		public string OrderBy {
			get => _orderBy;
			set {
				if (!(new string[] { "desc", "asc" }).Contains(value)) {
					throw new ArgumentOutOfRangeException(nameof(value));
				}
				_orderBy = value;
			}
		}

		// Returns the list with the Rotten Tomatoes rating included
		// Boolean
		// Default: false
		// Query parameter: with_rt_ratings
		public bool WithRtRatings { get; set; } = DEFAULT_WITH_RT_RATINGS;

		public string QueryString() {
			var parameters = new Dictionary<string, string>();

			if (Limit != DEFAULT_LIMIT) {
				parameters.Add("limit", Limit.ToString(CultureInfo.InvariantCulture));
			}
			if (Page != DEFAULT_PAGE) {
				parameters.Add("page", Page.ToString(CultureInfo.InvariantCulture));
			}
			if (Quality != DEFAULT_QUALITY) {
				parameters.Add("quality", Quality);
			}
			if (MinimumRating != DEFAULT_MINIMUM_RATING) {
				parameters.Add("minimum_rating", MinimumRating.ToString(CultureInfo.InvariantCulture));
			}
			if (QueryTerm != DEFAULT_QUERY_TERM) {
				parameters.Add("query_term", QueryTerm);
			}
			if (Genre != DEFAULT_GENRE) {
				parameters.Add("genre", Genre);
			}
			if (SortBy != DEFAULT_SORT_BY) {
				parameters.Add("sort_by", SortBy);
			}
			if (OrderBy != DEFAULT_ORDER_BY) {
				parameters.Add("order_by", OrderBy);
			}
			if (WithRtRatings != DEFAULT_WITH_RT_RATINGS) {
				parameters.Add("with_rt_ratings", WithRtRatings.ToString(CultureInfo.InvariantCulture).ToLowerInvariant());
			}

			if (parameters.Count == 0) {
				return "";
			}

			var query =
				"?" +
				string.Join("&",
					parameters
						.Select(x => $"{HttpUtility.UrlEncode(x.Key)}={HttpUtility.UrlEncode(x.Value)}")
					);

			return query;
		}
	}
}
