using System.Threading.Tasks;
using BencodeNET.Torrents;
using Oliver.Domain.Services;

namespace Oliver.Services.Interfaces {
	public interface ITorrentService {
		Task<TorrentVerification> VerifyTorrent(Torrent torrent, string folderName);
	}
}
