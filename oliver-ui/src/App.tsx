import { useState } from "react";
import { useStats, useMovies, useSyncStatus, useGenres } from "./hooks";
import type { MovieSummary } from "./api";
import "./App.css";

export default function App() {
	const [page, setPage] = useState(1);
	const [search, setSearch] = useState("");
	const [activeSearch, setActiveSearch] = useState("");
	const [genre, setGenre] = useState("");
	const [quality, setQuality] = useState("");

	const stats = useStats();
	const sync = useSyncStatus();
	const genres = useGenres();
	const movies = useMovies({ page, limit: 20, search: activeSearch || undefined, genre: genre || undefined, quality: quality || undefined });

	const handleSearch = (e: React.FormEvent) => {
		e.preventDefault();
		setActiveSearch(search);
		setPage(1);
	};

	return (
		<div className="app">
			<div className="container">
				<header>
					<h1>Oliver</h1>
					<p>YTS Movie Dataset Manager</p>
				</header>

				<div className="stats-grid">
					<StatCard label="Movies" value={stats.data?.movies} />
					<StatCard label="Torrents" value={stats.data?.torrents} />
					<StatCard label="Torrents Downloaded" value={stats.data?.torrentsDownloaded} />
					<StatCard label="Images Downloaded" value={stats.data?.imagesDownloaded} />
					<StatCard label="Genres" value={stats.data?.genres} />
					<StatCard label="Sync Page" value={sync.data?.lastCompletedPage ?? "—"} />
				</div>

				<section>
					<h2>Movie Browser</h2>

					<form className="filters" onSubmit={handleSearch}>
						<input
							type="text"
							placeholder="Search movies..."
							value={search}
							onChange={e => setSearch(e.target.value)}
							className="search-input"
						/>
						<button type="submit" className="btn">Search</button>

						<select value={genre} onChange={e => { setGenre(e.target.value); setPage(1); }} className="filter-select">
							<option value="">All Genres</option>
							{genres.data?.map(g => (
								<option key={g.name} value={g.name}>{g.name} ({g.count})</option>
							))}
						</select>

						<select value={quality} onChange={e => { setQuality(e.target.value); setPage(1); }} className="filter-select">
							<option value="">All Qualities</option>
							<option value="720p">720p</option>
							<option value="1080p">1080p</option>
							<option value="2160p">2160p</option>
							<option value="3D">3D</option>
						</select>

						{(activeSearch || genre || quality) && (
							<button type="button" className="btn btn-secondary" onClick={() => { setSearch(""); setActiveSearch(""); setGenre(""); setQuality(""); setPage(1); }}>
								Clear
							</button>
						)}
					</form>

					{movies.isLoading && <p className="loading">Loading movies...</p>}
					{movies.isError && <p className="error">Failed to load movies. Is the backend running?</p>}

					{movies.data && (
						<>
							<p className="result-count">
								{movies.data.totalCount.toLocaleString()} movies found — page {movies.data.page} of {movies.data.totalPages}
							</p>

							<div className="movie-grid">
								{movies.data.movies.map(movie => (
									<MovieCard key={movie.id} movie={movie} />
								))}
							</div>

							{movies.data.totalPages > 1 && (
								<div className="pagination">
									<button className="btn" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>Previous</button>
									<span className="page-info">Page {page} of {movies.data.totalPages}</span>
									<button className="btn" disabled={page >= movies.data.totalPages} onClick={() => setPage(p => p + 1)}>Next</button>
								</div>
							)}
						</>
					)}

					{movies.data?.movies.length === 0 && !movies.isLoading && (
						<p className="loading">No movies found. Run the Oliver backend to start syncing.</p>
					)}
				</section>
			</div>
		</div>
	);
}

function StatCard({ label, value }: { label: string; value: string | number | undefined }) {
	const display = value !== undefined ? (typeof value === "number" ? value.toLocaleString() : value) : "—";
	return (
		<div className="stat-card">
			<div className="value">{display}</div>
			<div className="label">{label}</div>
		</div>
	);
}

function MovieCard({ movie }: { movie: MovieSummary }) {
	const qualities = [...new Set(movie.torrents.map(t => t.quality).filter(Boolean))];

	return (
		<div className="movie-card">
			{movie.mediumCoverImage ? (
				<img src={movie.mediumCoverImage} alt={movie.title} loading="lazy" />
			) : (
				<div className="no-image">No Image</div>
			)}
			<div className="info">
				<div className="title" title={movie.title}>{movie.title}</div>
				<div className="meta">{movie.year} &middot; ★ {movie.rating}</div>
				{qualities.length > 0 && (
					<div className="qualities">
						{qualities.map(q => <span key={q} className="quality-badge">{q}</span>)}
					</div>
				)}
			</div>
		</div>
	);
}
