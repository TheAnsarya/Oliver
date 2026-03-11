using Microsoft.EntityFrameworkCore;
using Oliver.Domain;

namespace Oliver.Data;

public class OliverContext : DbContext {
	public OliverContext(DbContextOptions<OliverContext> options) : base(options) { }

	public DbSet<Movie> Movies => Set<Movie>();

	public DbSet<Genre> Genres => Set<Genre>();

	public DbSet<TorrentInfo> TorrentInfos => Set<TorrentInfo>();

	public DbSet<TorrentFileEntry> TorrentFileEntries => Set<TorrentFileEntry>();

	public DbSet<TorrentTracker> TorrentTrackers => Set<TorrentTracker>();

	public DbSet<SyncState> SyncStates => Set<SyncState>();

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		ArgumentNullException.ThrowIfNull(modelBuilder);
		modelBuilder.Entity<Movie>(e => {
			e.HasIndex(m => m.YtsId).IsUnique();
			e.HasMany(m => m.Torrents).WithOne(t => t.Movie).HasForeignKey(t => t.MovieId);
			e.HasMany(m => m.Genres).WithOne(g => g.Movie).HasForeignKey(g => g.MovieId);
		});

		modelBuilder.Entity<TorrentInfo>(e => {
			e.HasIndex(t => t.Hash);
			e.HasMany(t => t.Files).WithOne(f => f.TorrentInfo).HasForeignKey(f => f.TorrentInfoId);
			e.HasMany(t => t.Trackers).WithOne(tr => tr.TorrentInfo).HasForeignKey(tr => tr.TorrentInfoId);
		});

		modelBuilder.Entity<SyncState>(e => {
			e.HasIndex(s => s.Key).IsUnique();
		});
	}

	public override int SaveChanges() {
		SetTimestamps();
		return base.SaveChanges();
	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
		SetTimestamps();
		return base.SaveChangesAsync(cancellationToken);
	}

	private void SetTimestamps() {
		var entries = ChangeTracker.Entries<Entity>()
			.Where(e => e.State is EntityState.Added or EntityState.Modified);

		foreach (var entry in entries) {
			if (entry.State == EntityState.Added) {
				entry.Entity.CreatedDate = DateTime.UtcNow;
			}
			entry.Entity.UpdatedDate = DateTime.UtcNow;
		}
	}
}
