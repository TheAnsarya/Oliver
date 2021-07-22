using System.IO;
using System.Threading.Tasks;
using Oliver.Domain.Services;

namespace Oliver.Services.Interfaces {
	public interface IHashService {
		// MD5
		Task<byte[]> GetMD5(byte[] data);

		Task<byte[]> GetMD5(Stream stream);

		Task<string> GetMD5String(byte[] data);

		Task<string> GetMD5String(Stream stream);

		// SHA1
		Task<byte[]> GetSHA1(byte[] data);

		Task<byte[]> GetSHA1(Stream stream);

		Task<string> GetSHA1String(byte[] data);

		Task<string> GetSHA1String(Stream stream);

		// SHA256
		Task<byte[]> GetSHA256(byte[] data);

		Task<byte[]> GetSHA256(Stream stream);

		Task<string> GetSHA256String(byte[] data);

		Task<string> GetSHA256String(Stream stream);

		// Get all
		// TODO: do we need byte[] versions?
		Task<Hashes> GetAll(byte[] data);

		Task<Hashes> GetAll(Stream stream);
	}
}
