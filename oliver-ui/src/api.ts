const BASE = "/api";

export interface Stats {
	movies: number;
	torrents: number;
	torrentsDownloaded: number;
	imagesDownloaded: number;
	genres: number;
}

export interface TorrentSummary {
	quality: string | null;
	type: string | null;
	size: string | null;
	sizeBytes: number;
	seeds: number;
	peers: number;
	torrentFileDownloaded: boolean;
}

export interface MovieSummary {
	id: string;
	ytsId: number;
	title: string;
	year: number;
	rating: number;
	runtime: number;
	imdbCode: string | null;
	mediumCoverImage: string | null;
	largeCoverImage: string | null;
	language: string | null;
	imagesDownloaded: boolean;
	genres: string[];
	torrents: TorrentSummary[];
}

export interface MoviesResponse {
	movies: MovieSummary[];
	totalCount: number;
	page: number;
	limit: number;
	totalPages: number;
}

export interface SyncStatus {
	lastCompletedPage: string;
	syncStates: { key: string; value: string }[];
}

export interface GenreCount {
	name: string;
	count: number;
}

async function fetchJson<T>(url: string): Promise<T> {
	const res = await fetch(url);
	if (!res.ok) {
		throw new Error(`API error: ${res.status} ${res.statusText}`);
	}
	return res.json() as Promise<T>;
}

export function fetchStats(): Promise<Stats> {
	return fetchJson<Stats>(`${BASE}/stats`);
}

export function fetchMovies(params: {
	page?: number;
	limit?: number;
	search?: string;
	genre?: string;
	quality?: string;
}): Promise<MoviesResponse> {
	const query = new URLSearchParams();
	if (params.page) query.set("page", String(params.page));
	if (params.limit) query.set("limit", String(params.limit));
	if (params.search) query.set("search", params.search);
	if (params.genre) query.set("genre", params.genre);
	if (params.quality) query.set("quality", params.quality);
	return fetchJson<MoviesResponse>(`${BASE}/movies?${query}`);
}

export function fetchSyncStatus(): Promise<SyncStatus> {
	return fetchJson<SyncStatus>(`${BASE}/sync-status`);
}

export function fetchGenres(): Promise<GenreCount[]> {
	return fetchJson<GenreCount[]>(`${BASE}/genres`);
}
