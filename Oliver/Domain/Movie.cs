using System.Collections.Generic;
using System.Linq;
using Oliver.Domain.YTS.Responses;

namespace Oliver.Domain {
	public class Movie : Entity {
		public int YtsId { get; set; }

		public string Url { get; set; }

		public string ImdbCode { get; set; }

		public string Title { get; set; }

		public string TitleEnglish { get; set; }

		public string TitleLong { get; set; }

		public string Slug { get; set; }

		public int Year { get; set; }

		public decimal Rating { get; set; }

		public int Runtime { get; set; }

		public List<GenreString> Genres { get; set; }

		public string Summary { get; set; }

		public string DescriptionFull { get; set; }

		public string Synopsis { get; set; }

		public string YtTrailerCode { get; set; }

		public string Language { get; set; }

		public string MpaRating { get; set; }

		public string BackgroundImage { get; set; }

		public string BackgroundImageOriginal { get; set; }

		public string SmallCoverImage { get; set; }

		public string MediumCoverImage { get; set; }

		public string LargeCoverImage { get; set; }

		public string State { get; set; }

		public List<TorrentInfo> Torrents { get; set; }

		public string DateUploaded { get; set; }

		public long DateUploadedUnix { get; set; }

		public Movie() : base() { }

		public Movie(YtsMovie dto) {
			YtsId = dto.Id;
			Url = dto.Url;
			ImdbCode = dto.ImdbCode;
			Title = dto.Title;
			TitleEnglish = dto.TitleEnglish;
			TitleLong = dto.TitleLong;
			Slug = dto.Slug;
			Year = dto.Year;
			Rating = dto.Rating;
			Runtime = dto.Runtime;

			Genres =
				dto.Genres
					.Select(x => new GenreString(x, this))
					.ToList();

			Summary = dto.Summary;
			DescriptionFull = dto.DescriptionFull;
			Synopsis = dto.Synopsis;
			YtTrailerCode = dto.YtTrailerCode;
			Language = dto.Language;
			MpaRating = dto.MpaRating;
			BackgroundImage = dto.BackgroundImage;
			BackgroundImageOriginal = dto.BackgroundImageOriginal;
			SmallCoverImage = dto.SmallCoverImage;
			MediumCoverImage = dto.MediumCoverImage;
			LargeCoverImage = dto.LargeCoverImage;
			State = dto.State;

			Torrents =
				dto.Torrents
					.Select(x => new TorrentInfo(x, this))
					.ToList();

			DateUploaded = dto.DateUploaded;
			DateUploadedUnix = dto.DateUploadedUnix;
		}
	}
}
