using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Oliver.Domain;

namespace Oliver.Data {
	public class OliverContext : DbContext {
		protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=oliver.db");

		public override int SaveChanges() {
			var entries =
				ChangeTracker
					.Entries()
					.Where(x => x.Entity is Entity && (x.State == EntityState.Added || x.State == EntityState.Modified));

			foreach (var entry in entries) {
				var entity = (Entity)entry.Entity;

				if (entry.State == EntityState.Added) {
					entity.CreatedDate = DateTime.Now;
				}

				entity.UpdatedDate = DateTime.Now;
			}

			return base.SaveChanges();
		}

		public DbSet<Movie> Movies { get; set; }

		public DbSet<TorrentInfo> TorrentInfos { get; set; }

		public DbSet<GenreString> GenreStrings { get; set; }

		public DbSet<TorrentFile> TorrentFiles { get; set; }
	}
}
