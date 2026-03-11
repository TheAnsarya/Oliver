using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Oliver.Data;

namespace Oliver.Services;

public partial class FileMatchingService(IServiceScopeFactory scopeFactory, ILogger<FileMatchingService> logger) {
	[GeneratedRegex(@"^(.+?)[\.\s\-_]+(\d{4})[\.\s\-_]", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
	private static partial Regex TitleYearPattern();

	[GeneratedRegex(@"(720p|1080p|2160p|4K)", RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
	private static partial Regex QualityPattern();

	public async Task<FileMatchResult> MatchAllAsync(CancellationToken ct = default) {
		using var scope = scopeFactory.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<OliverContext>();

		var unmatchedFiles = await db.LocalFiles
			.Where(f => f.MatchedMovieId == null)
			.ToListAsync(ct);

		if (unmatchedFiles.Count == 0) {
			logger.LogInformation("No unmatched local files to process");
			return new FileMatchResult();
		}

		var result = new FileMatchResult { TotalProcessed = unmatchedFiles.Count };

		foreach (var file in unmatchedFiles) {
			ct.ThrowIfCancellationRequested();

			var parsed = ParseFilename(file.FileName);
			if (parsed is null) {
				result.ParseFailed++;
				continue;
			}

			// Try exact title + year match (SQLite LIKE is case-insensitive by default)
			var movie = await db.Movies
				.Include(m => m.Torrents)
				.FirstOrDefaultAsync(m =>
					EF.Functions.Like(m.Title, parsed.Title) && m.Year == parsed.Year, ct);

			if (movie is null) {
				// Try contains match
				var pattern = $"%{parsed.Title}%";
				movie = await db.Movies
					.Include(m => m.Torrents)
					.FirstOrDefaultAsync(m =>
						EF.Functions.Like(m.Title, pattern) && m.Year == parsed.Year, ct);
			}

			if (movie is null) {
				result.NoMatch++;
				continue;
			}

			file.MatchedMovieId = movie.Id;

			// Try to match specific torrent by quality
			if (parsed.Quality is not null) {
				var torrent = movie.Torrents.FirstOrDefault(t =>
					string.Equals(t.Quality, parsed.Quality, StringComparison.OrdinalIgnoreCase));
				if (torrent is not null) {
					file.MatchedTorrentId = torrent.Id;
				}
			}

			result.Matched++;
		}

		await db.SaveChangesAsync(ct);

		logger.LogInformation("File matching complete: {Matched} matched, {NoMatch} no match, {ParseFailed} parse failed out of {Total}",
			result.Matched, result.NoMatch, result.ParseFailed, result.TotalProcessed);

		return result;
	}

	internal static ParsedFilename? ParseFilename(string filename) {
		var match = TitleYearPattern().Match(filename);
		if (!match.Success) {
			return null;
		}

		var rawTitle = match.Groups[1].Value
			.Replace('.', ' ')
			.Replace('_', ' ')
			.Replace('-', ' ')
			.Trim();

		if (!int.TryParse(match.Groups[2].Value, out var year) || year < 1900 || year > 2100) {
			return null;
		}

		var qualityMatch = QualityPattern().Match(filename);
		var quality = qualityMatch.Success ? qualityMatch.Groups[1].Value : null;
		if (string.Equals(quality, "4K", StringComparison.OrdinalIgnoreCase)) {
			quality = "2160p";
		}

		return new ParsedFilename { Title = rawTitle, Year = year, Quality = quality };
	}
}

internal class ParsedFilename {
	public string Title { get; set; } = string.Empty;

	public int Year { get; set; }

	public string? Quality { get; set; }
}

public class FileMatchResult {
	public int TotalProcessed { get; set; }

	public int Matched { get; set; }

	public int NoMatch { get; set; }

	public int ParseFailed { get; set; }
}
