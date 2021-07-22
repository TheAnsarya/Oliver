using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Oliver.Domain;

namespace Oliver.Data {
	public class OliverContext : DbContext {
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Data Source=oliver.db");

		public override int SaveChanges() {
			SaveChangesHelper();
			return base.SaveChanges();
		}

		public override int SaveChanges(bool acceptAllChangesOnSuccess) {
			SaveChangesHelper();
			return base.SaveChanges(acceptAllChangesOnSuccess);
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
			SaveChangesHelper();
			return base.SaveChangesAsync(cancellationToken);
		}

		public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) {
			SaveChangesHelper();
			return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
		}

		private void SaveChangesHelper() {
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
		}

		public DbSet<Movie> Movies { get; set; }

		public DbSet<TorrentInfo> TorrentInfos { get; set; }

		public DbSet<GenreString> GenreStrings { get; set; }

		public DbSet<TorrentFile> TorrentFiles { get; set; }

		public DbSet<TorrentDataFile> TorrentDataFiles { get; set; }

		public DbSet<DataFile> DataFiles { get; set; }
	}
}
