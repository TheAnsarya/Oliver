// StarFire Five
// © 2021 Kikoda & Andy Hubbard
// All rights reserved

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Oliver.Domain;

namespace Oliver.Data.Mappings {
	public static class TorrentDataFileMappings {
		internal static void Configure(EntityTypeBuilder<TorrentDataFile> type) {
			//type.HasOne(x =>x.DataFile)
			//	.WithOne(x=>x.TorrentDataFile)
			//	.HasForeignKey<DataFile>(x=>x.)
		}
	}
}
