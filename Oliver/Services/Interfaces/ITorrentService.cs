using System.Threading.Tasks;
using BencodeNET.Torrents;
using Oliver.Domain;
using Oliver.Domain.Services;
using Oliver.Domain.YTS.Requests;
using Oliver.Domain.YTS.Responses;

namespace Oliver.Services.Interfaces {
	public interface ITorrentService {
		Task<TorrentVerification> VerifyTorrent(Torrent torrent, string folderName);
	}
}
