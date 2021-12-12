using System;
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
		private const int ChunkLength = 32 * 1024;

		// MD5
		public async Task<byte[]> GetMD5(byte[] data) {
			using var mem = new MemoryStream(data);
			return await GetMD5(mem);
		}

		public async Task<byte[]> GetMD5(Stream stream) {
			using var hasher = MD5.Create();
			var hash = await hasher.ComputeHashAsync(stream);

			return hash;
		}

		public async Task<string> GetMD5String(byte[] data) {
			using var mem = new MemoryStream(data);
			return await GetMD5String(mem);
		}

		public async Task<string> GetMD5String(Stream stream) {
			var hash = await GetMD5(stream);

			return hash.ToHexString();
		}

		// SHA1
		public async Task<byte[]> GetSHA1(byte[] data) {
			using var mem = new MemoryStream(data);
			return await GetSHA1(mem);
		}

		public async Task<byte[]> GetSHA1(Stream stream) {
			using var hasher = SHA1.Create();
			var hash = await hasher.ComputeHashAsync(stream);

			return hash;
		}

		public async Task<string> GetSHA1String(byte[] data) {
			using var mem = new MemoryStream(data);
			return await GetSHA1String(mem);
		}

		public async Task<string> GetSHA1String(Stream stream) {
			var hash = await GetSHA1(stream);

			return hash.ToHexString();
		}

		// SHA256
		public async Task<byte[]> GetSHA256(byte[] data) {
			using var mem = new MemoryStream(data);
			return await GetSHA256(mem);
		}

		public async Task<byte[]> GetSHA256(Stream stream) {
			using var hasher = SHA256.Create();
			var hash = await hasher.ComputeHashAsync(stream);

			return hash;
		}

		public async Task<string> GetSHA256String(byte[] data) {
			using var mem = new MemoryStream(data);
			return await GetSHA256String(mem);
		}

		public async Task<string> GetSHA256String(Stream stream) {
			var hash = await GetSHA256(stream);

			return hash.ToHexString();
		}

		// Get all
		// TODO: do we need byte[] versions?
		public async Task<Hashes> GetAll(byte[] data) {
			using var mem = new MemoryStream(data);
			return await GetAll(mem);
		}

		public async Task<Hashes> GetAll(Stream stream) {
			if (stream == null) {
				throw new ArgumentNullException(nameof(stream));
			}

			using var md5Hasher = MD5.Create();
			using var sha1Hasher = SHA1.Create();
			using var sha256Hasher = SHA256.Create();

			var buffer = ArrayPool<byte>.Shared.Rent(ChunkLength);
			int read;

			while ((read = await stream.ReadAsync(buffer.AsMemory(0, ChunkLength))) > 0) {
				// TODO: check to make sure reutrn values match what we expect 
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
