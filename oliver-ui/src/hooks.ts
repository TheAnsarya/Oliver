import { useQuery, keepPreviousData } from "@tanstack/react-query";
import { fetchStats, fetchMovies, fetchSyncStatus, fetchGenres } from "./api";

export function useStats() {
	return useQuery({
		queryKey: ["stats"],
		queryFn: fetchStats,
		refetchInterval: 10_000,
	});
}

export function useMovies(params: {
	page?: number;
	limit?: number;
	search?: string;
	genre?: string;
	quality?: string;
}) {
	return useQuery({
		queryKey: ["movies", params],
		queryFn: () => fetchMovies(params),
		placeholderData: keepPreviousData,
	});
}

export function useSyncStatus() {
	return useQuery({
		queryKey: ["sync-status"],
		queryFn: fetchSyncStatus,
		refetchInterval: 10_000,
	});
}

export function useGenres() {
	return useQuery({
		queryKey: ["genres"],
		queryFn: fetchGenres,
	});
}
