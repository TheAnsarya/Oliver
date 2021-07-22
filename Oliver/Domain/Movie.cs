using System;
using System.Collections.Generic;
using System.Linq;
using Oliver.Domain.YTS.Responses;

namespace Oliver.Domain {
	public class Movie : Entity, IEquatable<Movie> {
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

		public string DateUploaded { get; set; }

		public long DateUploadedUnix { get; set; }

		// Link Properties

		public virtual List<TorrentInfo> Torrents { get; set; }

		public virtual List<GenreString> Genres { get; set; }

		// Constructors

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

		// Equality

		public override bool Equals(object obj) {
			return Equals(obj as Movie);
		}

		public bool Equals(Movie other) {
			return other != null &&
				   YtsId == other.YtsId &&
				   Url == other.Url &&
				   ImdbCode == other.ImdbCode &&
				   Title == other.Title &&
				   TitleEnglish == other.TitleEnglish &&
				   TitleLong == other.TitleLong &&
				   Slug == other.Slug &&
				   Year == other.Year &&
				   Rating == other.Rating &&
				   Runtime == other.Runtime &&
				   EqualityComparer<List<GenreString>>.Default.Equals(Genres, other.Genres) &&
				   Summary == other.Summary &&
				   DescriptionFull == other.DescriptionFull &&
				   Synopsis == other.Synopsis &&
				   YtTrailerCode == other.YtTrailerCode &&
				   Language == other.Language &&
				   MpaRating == other.MpaRating &&
				   BackgroundImage == other.BackgroundImage &&
				   BackgroundImageOriginal == other.BackgroundImageOriginal &&
				   SmallCoverImage == other.SmallCoverImage &&
				   MediumCoverImage == other.MediumCoverImage &&
				   LargeCoverImage == other.LargeCoverImage &&
				   State == other.State &&
				   EqualityComparer<List<TorrentInfo>>.Default.Equals(Torrents, other.Torrents) &&
				   DateUploaded == other.DateUploaded &&
				   DateUploadedUnix == other.DateUploadedUnix;
		}

		public override int GetHashCode() {
			var hash = new HashCode();
			hash.Add(YtsId);
			hash.Add(Url);
			hash.Add(ImdbCode);
			hash.Add(Title);
			hash.Add(TitleEnglish);
			hash.Add(TitleLong);
			hash.Add(Slug);
			hash.Add(Year);
			hash.Add(Rating);
			hash.Add(Runtime);
			hash.Add(Genres);
			hash.Add(Summary);
			hash.Add(DescriptionFull);
			hash.Add(Synopsis);
			hash.Add(YtTrailerCode);
			hash.Add(Language);
			hash.Add(MpaRating);
			hash.Add(BackgroundImage);
			hash.Add(BackgroundImageOriginal);
			hash.Add(SmallCoverImage);
			hash.Add(MediumCoverImage);
			hash.Add(LargeCoverImage);
			hash.Add(State);
			hash.Add(Torrents);
			hash.Add(DateUploaded);
			hash.Add(DateUploadedUnix);
			return hash.ToHashCode();
		}

		public static bool operator ==(Movie left, Movie right) {
			return EqualityComparer<Movie>.Default.Equals(left, right);
		}

		public static bool operator !=(Movie left, Movie right) {
			return !(left == right);
		}

		// Update

		public void Update(Movie other) {
			if (other == null) {
				throw new ArgumentNullException(nameof(other));
			} else if (YtsId != other.YtsId) {
				throw new ArgumentException($"Cannot merge {nameof(Movie)} :: {nameof(YtsId)} must match", nameof(other));
			}

			Url = other.Url;
			ImdbCode = other.ImdbCode;
			Title = other.Title;
			TitleEnglish = other.TitleEnglish;
			TitleLong = other.TitleLong;
			Slug = other.Slug;
			Year = other.Year;
			Rating = other.Rating;
			Runtime = other.Runtime;
			Summary = other.Summary;
			DescriptionFull = other.DescriptionFull;
			Synopsis = other.Synopsis;
			YtTrailerCode = other.YtTrailerCode;
			Language = other.Language;
			MpaRating = other.MpaRating;
			BackgroundImage = other.BackgroundImage;
			BackgroundImageOriginal = other.BackgroundImageOriginal;
			SmallCoverImage = other.SmallCoverImage;
			MediumCoverImage = other.MediumCoverImage;
			LargeCoverImage = other.LargeCoverImage;
			State = other.State;
			DateUploaded = other.DateUploaded;
			DateUploadedUnix = other.DateUploadedUnix;

			var tempGenres = new List<GenreString>(Genres);
			foreach (var genre in tempGenres) {
				if (!other.Genres.Any(x => x == genre)) {
					Genres.Remove(genre);
				}
			}

			foreach (var genre in other.Genres) {
				if (!Genres.Any(x => x == genre)) {
					Genres.Add(genre);
				}
			}

			var tempTorrents = new List<TorrentInfo>(Torrents);
			foreach (var torrent in other.Torrents) {
				if (!Torrents.Any(x => x == torrent)) {
					var found = Torrents.Where(x => x.Hash == torrent.Hash).FirstOrDefault();

					if (found == null) {
						Torrents.Add(torrent);
					} else {
						found.Update(torrent);
					}
				}
			}

			foreach (var torrent in tempTorrents) {
				if (!other.Torrents.Any(x => x == torrent)) {
					torrent.Current = false;
				}
			}
		}
	}
}
