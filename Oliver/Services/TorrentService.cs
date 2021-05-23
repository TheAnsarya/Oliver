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
using Oliver.Domain.Services;
using Oliver.Domain.YTS.Requests;
using Oliver.Domain.YTS.Responses;
using Oliver.Extensions;
using Oliver.Services.Interfaces;

namespace Oliver.Services {
	public class TorrentService : ITorrentService {
		private const int PIECE_HASH_STRING_SIZE = 40;

		private readonly IConfiguration _config;

		private readonly ILogger _logger;

		private readonly IHashService _hasher;

		public TorrentService(IConfiguration config, ILogger logger, IHashService hasher) {
			_config = config;
			_logger = logger;
			_hasher = hasher;
		}

		// We are assuming the that torrent, if analyzed, is properly represented in the torrentdatafiles
		// TODO: verify the above assumption
		public async Task<TorrentVerification> VerifyTorrent(TorrentFile torrentFile) {
			/*
			 * [x]	make sure analyzed
			 * [x]	verify all torrentdatafiles are matched with a datafile
			 * [x]	load torrent from bencode
			 * [ ]	build files in torrent datastructure
			 *				******* use torrentdata files instead
			 * [ ]	make sure all datafiles still exist and sizes still match
			 * [ ]	match files from torrent to datafiles
			 *				******* use torrentdata files instead
			 * [ ]	(verify full match)
			 *				******* use torrentdata files instead
			 * [ ]	generate pieces hash from datafiles
			 * [ ]	add file-part list to pieces as made (which file, start of read, end of read)
			 * [ ]	compare pieces
			 * [ ]		all match == torrentfile is verified
			 * [ ]		is each file matched
			 * [ ]
			 * [ ]
			 */

			if (torrentFile == null) {
				throw new ArgumentNullException(nameof(torrentFile));
			}

			if (torrentFile.AnalyzedStatus == TorrentAnalyzedStatus.NotAnalyzed) {
				return new TorrentVerification {
					Status = TorrentVerificationStatus.NotAnalyzed
				};
			}

			if (torrentFile.AnalyzedStatus == TorrentAnalyzedStatus.NotParsable) {
				return new TorrentVerification {
					Status = TorrentVerificationStatus.TorrentCannotBeParsed
				};
			}

			if (torrentFile.TorrentDataFiles.Any(x => x.DataFile == null)) {
				return new TorrentVerification {
					Status = TorrentVerificationStatus.MissingDataFiles
				};
			}

			// Make sure files exist and sizes are still the same
			var badDataFiles =
				torrentFile.TorrentDataFiles
					.Select(x => x.DataFile)
					.Where(x => !File.Exists(x.FilePath) || (new FileInfo(x.FilePath).Length != x.Size))
					.ToList();

			if (badDataFiles.Count > 0) {
				return new TorrentVerification {
					Status = TorrentVerificationStatus.DataFilesNotFound,
					BadDataFiles = badDataFiles,
				};
			}


		}

		public async Task<TorrentVerification> VerifyTorrent(Torrent torrent, string folderName) {
			if (torrent == null) {
				throw new ArgumentNullException(nameof(torrent));
			} else if (torrent.FileMode != TorrentFileMode.Multi) {
				// TODO: Implement single file checking although we aren't using it here
				throw new ArgumentException("Only handles multi-file torrents for now.", nameof(torrent));
			} else if (!Directory.Exists(folderName)) {
				throw new ArgumentException("Directory does not exist.", nameof(folderName));
			}

			if (!folderName.EndsWith(Path.DirectorySeparatorChar)) {
				folderName = $"{folderName}{Path.DirectorySeparatorChar}";
			}

			var filenames = torrent.Files.Select(x => Path.Combine(folderName, x.FullPath));

			var filenameChecks =
				filenames
					.Select(x => File.Exists(x))
					.ToArray();

			var filesInFolder = Directory.GetFiles(folderName, "*", SearchOption.AllDirectories);

			var unknownFiles =
				filesInFolder
					.Where(x => !filenames.Contains(x))
					.Select(x => x.Substring(folderName.Length))
					.ToArray();

			var pieceHashes =
				torrent.PiecesAsHexString
					.Split(PIECE_HASH_STRING_SIZE);


			foreach (var filename in filenames) {

			}




			return new TorrentVerification {
				Filenames = filenameChecks,
				Files = ,
				UnknownFiles = unknownFiles,
				Pieces = ,
			};
		}

		private IEnumerable<byte[]> GetDataPieces(string[] filenames, int pieceLength) {
			if (filenames == null) {
				throw new ArgumentNullException(nameof(filenames));
			} else if (pieceLength < 1) {
				throw new ArgumentOutOfRangeException("Length must be greater than zero.", nameof(pieceLength));
			}



			for (int i = 0; i < input.Length; i += length) {
				yield return input.Substring(i, Math.Min(length, input.Length - i));
			}

		}

	}
}
