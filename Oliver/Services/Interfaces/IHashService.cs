﻿using System.IO;
using System.Threading.Tasks;
using Oliver.Domain.Services;

namespace Oliver.Services.Interfaces {
	public interface IHashService {
		Task<string> GetMD5(Stream stream);

		Task<string> GetSHA1(Stream stream);

		Task<string> GetSHA256(Stream stream);

		Task<Hashes> GetAll(Stream stream);
	}
}