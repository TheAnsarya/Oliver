using System.Buffers;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Oliver.Domain.Services;
using Oliver.Extensions;
using Oliver.Services.Interfaces;

namespace Oliver.Services {
	public class HashService : IHashService {
		// Read stream in 32k chunks
		private const int CHUNK_LENGTH = 32 * 1024;

		public async Task<string> GetMD5(Stream stream) {
			using var hasher = MD5.Create();
			var hash = await hasher.ComputeHashAsync(stream);

			return hash.ToHexString();
		}

		public async Task<string> GetSHA1(Stream stream) {
			using var hasher = SHA1.Create();
			var hash = await hasher.ComputeHashAsync(stream);

			return hash.ToHexString();
		}

		public async Task<string> GetSHA256(Stream stream) {
			using var hasher = SHA256.Create();
			var hash = await hasher.ComputeHashAsync(stream);

			return hash.ToHexString();
		}

		public async Task<Hashes> GetAll(Stream stream) {
			using var md5Hasher = MD5.Create();
			using var sha1Hasher = SHA1.Create();
			using var sha256Hasher = SHA256.Create();

			var buffer = ArrayPool<byte>.Shared.Rent(CHUNK_LENGTH);
			int read;

			while ((read = await stream.ReadAsync(buffer, 0, CHUNK_LENGTH)) > 0) {
				md5Hasher.TransformBlock(buffer, 0, read, buffer, 0);
				sha1Hasher.TransformBlock(buffer, 0, read, buffer, 0);
				sha256Hasher.TransformBlock(buffer, 0, read, buffer, 0);
			}

			var md5 = md5Hasher.TransformFinalBlock(buffer, 0, read).ToHexString();
			var sha1 = sha1Hasher.TransformFinalBlock(buffer, 0, read).ToHexString();
			var sha256 = sha256Hasher.TransformFinalBlock(buffer, 0, read).ToHexString();

			ArrayPool<byte>.Shared.Return(buffer);

			return new Hashes {
				MD5 = md5,
				SHA1 = sha1,
				SHA256 = sha256
			};
		}
	}
}
