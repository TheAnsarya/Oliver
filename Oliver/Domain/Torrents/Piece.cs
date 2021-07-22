using System.Collections.Generic;
using BencodeNET.Torrents;
using Oliver.Extensions;

namespace Oliver.Domain.Torrents {
	public class Piece {
		public byte[] Hash { get; set; }

		public string HashString => Hash.ToHexString();
	}

	public List<Piece> GetListFromTorrent(Torrent torrent) {
		torrent.Pieces
	}
}
