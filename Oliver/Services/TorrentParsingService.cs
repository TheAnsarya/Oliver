using BencodeNET.Parsing;
using BencodeNET.Torrents;
using Oliver.Domain;

namespace Oliver.Services;

public class TorrentParsingService(ILogger<TorrentParsingService> logger) {
	public TorrentParseResult? Parse(string filePath) {
		try {
			var parser = new TorrentParser();
			using var stream = File.OpenRead(filePath);
			var torrent = parser.Parse(stream);

			var result = new TorrentParseResult {
				InfoHash = torrent.GetInfoHash(),
				Name = torrent.DisplayName,
				PieceLength = torrent.PieceSize,
				CreationDate = torrent.CreationDate,
				Comment = torrent.Comment,
				MagnetLink = torrent.GetMagnetLink(),
			};

			// Extract files
			if (torrent.Files is not null) {
				foreach (var file in torrent.Files) {
					result.Files.Add(new TorrentFileEntryData {
						Path = file.FullPath,
						Size = file.FileSize,
					});
				}
			} else if (torrent.File is not null) {
				// Single-file torrent
				result.Files.Add(new TorrentFileEntryData {
					Path = torrent.File.FileName ?? torrent.DisplayName ?? "unknown",
					Size = torrent.File.FileSize,
				});
			}

			result.TotalFileSize = result.Files.Sum(f => f.Size);
			result.FileCount = result.Files.Count;

			// Extract trackers
			if (torrent.Trackers is not null) {
				var tier = 0;
				foreach (var trackerList in torrent.Trackers) {
					foreach (var trackerUrl in trackerList) {
						if (!string.IsNullOrWhiteSpace(trackerUrl)) {
							result.Trackers.Add(new TorrentTrackerData {
								Url = trackerUrl,
								Tier = tier,
							});
						}
					}
					tier++;
				}
			}

			logger.LogDebug("Parsed torrent {Name}: {FileCount} files, {TrackerCount} trackers, hash={Hash}",
				result.Name, result.FileCount, result.Trackers.Count, result.InfoHash);

			return result;
		} catch (Exception ex) {
			logger.LogWarning(ex, "Failed to parse torrent file: {Path}", filePath);
			return null;
		}
	}
}

public class TorrentParseResult {
	public string? InfoHash { get; set; }

	public string? Name { get; set; }

	public long TotalFileSize { get; set; }

	public int FileCount { get; set; }

	public long PieceLength { get; set; }

	public DateTime? CreationDate { get; set; }

	public string? Comment { get; set; }

	public string? MagnetLink { get; set; }

	public List<TorrentFileEntryData> Files { get; set; } = [];

	public List<TorrentTrackerData> Trackers { get; set; } = [];
}

public class TorrentFileEntryData {
	public string Path { get; set; } = string.Empty;

	public long Size { get; set; }
}

public class TorrentTrackerData {
	public string Url { get; set; } = string.Empty;

	public int Tier { get; set; }
}
