using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
