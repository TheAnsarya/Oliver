using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BencodeNET.Torrents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oliver.Constants;
using Oliver.Domain;
using Oliver.Domain.Services;
using Oliver.Domain.Streams;
using Oliver.Domain.Torrents;
using Oliver.Extensions;
using Oliver.Services.Interfaces;

namespace Oliver.Services {
	public class TorrentService : ITorrentService {
		private const int PIECE_HASH_STRING_SIZE = 40;


		private IConfiguration Config { get; }

		private ILogger Logger { get; }

		private IHashService Hasher { get; }

		public TorrentService(IConfiguration config, ILogger logger, IHashService hasher) {
			Config = config;
			Logger = logger;
			Hasher = hasher;
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
			 * [x]	generate pieces hash from datafiles
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
			}

			if (folderName == null) {
				throw new ArgumentNullException(nameof(folderName));
			}

			if (!Directory.Exists(folderName)) {
				throw new ArgumentException("Directory does not exist.", nameof(folderName));
			}

			// TODO: Implement single file checking although we aren't using it here
			if (torrent.FileMode != TorrentFileMode.Multi) {
				throw new ArgumentException("Only handles multi-file torrents for now.", nameof(torrent));
			}

			// Ensure folder path ends with separator, so "c:\working\" not "c:\working"
			if (!folderName.EndsWith(Path.DirectorySeparatorChar)) {
				folderName = $"{folderName}{Path.DirectorySeparatorChar}";
			}

			// Check for missing and extra (unknown) files in the folder
			var filenames = torrent.Files.Select(x => Path.Combine(folderName, x.FullPath));
			var filesInFolder = Directory.GetFiles(folderName, "*", SearchOption.AllDirectories);
			//var files =
			//	torrent.Files
			//		.Select(x => new {
			//			ExpectedSize = x.FileSize,
			//			FileName = x.FullPath,
			//			FileNameFull = Path.Combine(folderName, x.FullPath),

			//		})
			//		.Select(x => new FileMatch {
			//			 ExpectedSize = x.FileSize,
			//			 FileName = x.FullPath,
			//			 FileNameFull = Path.Combine(folderName, x.FullPath),

			//		 });

			var unknownFiles =
				filesInFolder
					.Where(x => !filenames.Contains(x))
					.Select(x => x[folderName.Length..])
					.ToArray();

			var missingFiles =
				filenames
					.Where(x => !filesInFolder.Contains(x))
					.Select(x => x[folderName.Length..])
					.ToArray();

			var missingFilesFlagged =
				filenames
					.Select(x => filesInFolder.Contains(x))
					.ToArray();

			var badFiles = torrent.Files.Select(x => x.FileSize);

			//var pieceStringHashes =
			//	torrent.PiecesAsHexString
			//		.Split(PIECE_HASH_STRING_SIZE);

			// TODO: should we switch to Memory<byte> and get rid of PieceHash?
			var torrentPieces = new PieceSet(new PieceSize(torrent.PieceSize), torrent.Pieces);
			var testedPieces = await GetDataPieces(filenames.ToArray(), new PieceSize(torrent.PieceSize));

			if (torrentPieces.Pieces.Count != testedPieces.Pieces.Count) {

			} else if (torrent.Pieces.IsSame(testedPieces.FullHash)) {

			} else {

			}





			return new TorrentVerification {
				Filenames = missingFilesFlagged,
				//Files = ,
				UnknownFiles = unknownFiles,
				//Pieces = ,
			};
		}

		private async Task<PieceSet> GetDataPieces(string[] filenames, PieceSize size) {
			if (filenames == null) {
				throw new ArgumentNullException(nameof(filenames));
			}

			var files = FileNamesToInfo(filenames);

			return await GetDataPieces(files, size);
		}

		private async Task<PieceSet> GetDataPieces(List<FileInfo> files, PieceSize size) {
			if (files == null) {
				throw new ArgumentNullException(nameof(files));
			}

			if (files.Any(x => !x.Exists)) {
				throw new ArgumentException("Not all of the files exist.", nameof(files));
			}

			var totalFileLength = files.Sum(x => x.Length);
			var piecesSet = new PieceSet(size, size.ExpectedPieces(totalFileLength));

			using var stream = new MuliFileStream(files);
			var buffer = ArrayPool<byte>.Shared.Rent(size.Size);

			while (!stream.Done) {
				var read = await stream.ReadAsync(buffer.AsMemory(0, size.Size));

				if (read != 0) {
					var hash = await Hasher.GetSHA1(read < size.Size ? buffer.AsMemory(0, read).ToArray() : buffer);
					piecesSet.Pieces.Add(new PieceHash(hash));
				}
			}

			ArrayPool<byte>.Shared.Return(buffer);

			return piecesSet;
		}

		private static List<FileInfo> FileNamesToInfo(string[] filenames) => filenames.Select(x => new FileInfo(x)).ToList();
	}
}
