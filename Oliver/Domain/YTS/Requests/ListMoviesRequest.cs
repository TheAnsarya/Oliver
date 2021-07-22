using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Oliver.Domain.YTS.Requests {
	// TODO: A lot of this class seems templatable.
	public class ListMoviesRequest {
		public const int DefaultLimit = 20;
		public const int DefaultPage = 20;
		public const string DefaultQuality = "All";
		public const int DefaultMinimumRating = 0;
		public const string DefaultQueryTerm = "0";
		public const string DefaultGenre = "All";
		public const string DefaultSortBy = "date_added";
		public const string DefaultOrderBy = "desc";
		public const bool DefaultWithRTRatings = false;

		// The limit of results per page that has been set
		// between 1 - 50 (inclusive)
		// Default: 20
		// Query parameter: limit
		private int _limit = DefaultLimit;
		public int Limit {
			get => _limit;
			set {
				if (value is < 1 or > 50) {
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				_limit = value;
			}
		}

		// Used to see the next page of movies, eg limit=15 and page = 2 will show you movies 15-30
		// Integer(Unsigned)
		// Default: 1
		// Query parameter: page
		private int _page = DefaultPage;
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
		private string _quality = DefaultQuality;
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
		private int _minimumRating = DefaultMinimumRating;
		public int MinimumRating {
			get => _minimumRating;
			set {
				if (value is < 0 or > 9) {
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				_minimumRating = value;
			}
		}

		// Used for movie search, matching on: Movie Title/IMDb Code, Actor Name/IMDb Code, Director Name/IMDb Code
		// String
		// Default: "0"
		// Query parameter: query_term
		public string QueryTerm { get; set; } = DefaultQueryTerm;

		// Used to filter by a given genre (See http://www.imdb.com/genre/ for full list)
		// String
		// Default: "All"
		// Query parameter: genre
		public string Genre { get; set; } = DefaultGenre;

		// Sorts the results by choosen value
		// String (title, year, rating, peers, seeds, download_count, like_count, date_added)
		// Default: "date_added"
		// Query parameter: sort_by
		private string _sortBy = DefaultSortBy;
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
		private string _orderBy = DefaultOrderBy;
		public string OrderBy {
			get => _orderBy;
			set {
				if (value is not "desc" and not "asc") {
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				_orderBy = value;
			}
		}

		// Returns the list with the Rotten Tomatoes rating included
		// Boolean
		// Default: false
		// Query parameter: with_rt_ratings
		public bool WithRtRatings { get; set; } = DefaultWithRTRatings;

		public string QueryString() {
			var parameters = new Dictionary<string, string>();

			if (Limit != DefaultLimit) {
				parameters.Add("limit", Limit.ToString(CultureInfo.InvariantCulture));
			}

			if (Page != DefaultPage) {
				parameters.Add("page", Page.ToString(CultureInfo.InvariantCulture));
			}

			if (Quality != DefaultQuality) {
				parameters.Add("quality", Quality);
			}

			if (MinimumRating != DefaultMinimumRating) {
				parameters.Add("minimum_rating", MinimumRating.ToString(CultureInfo.InvariantCulture));
			}

			if (QueryTerm != DefaultQueryTerm) {
				parameters.Add("query_term", QueryTerm);
			}

			if (Genre != DefaultGenre) {
				parameters.Add("genre", Genre);
			}

			if (SortBy != DefaultSortBy) {
				parameters.Add("sort_by", SortBy);
			}

			if (OrderBy != DefaultOrderBy) {
				parameters.Add("order_by", OrderBy);
			}

			if (WithRtRatings != DefaultWithRTRatings) {
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
