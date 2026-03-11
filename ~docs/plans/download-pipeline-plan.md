# Download Pipeline Plan

## Goal

Build a complete local copy of the YTS movie dataset:

- All movie metadata in a SQLite database
- All `.torrent` files on disk
- All cover images and background images on disk

## Phases

### Phase 1: Core Pipeline (Complete)

- [x] .NET 10 Worker Service project setup (upgraded from .NET 9)
- [x] Domain models (Movie, Genre, TorrentInfo, SyncState)
- [x] EF Core 10 + SQLite database context
- [x] YTS API client with pagination and retry logic
- [x] Download service for torrents and images with retry logic
- [x] Background worker orchestrating the full pipeline
- [x] Resume support via SyncState tracking
- [x] Structured folder layout for downloads
- [x] Parallel downloads with SemaphoreSlim (configurable concurrency)
- [x] Exponential backoff retry on all HTTP operations
- [x] HTTP connection pooling via SocketsHttpHandler
- [x] Batch processing with chunked DB saves

### Phase 2: Data Verification

- [ ] Verify all movies have been synced (compare DB count vs API total)
- [ ] Verify all torrent files downloaded and are valid bencode
- [ ] Verify all images downloaded and are valid JPEG/PNG
- [ ] Parse torrent files with BencodeNET for file lists and metadata
- [ ] Store parsed torrent metadata in database

### Phase 3: Data Enrichment

- [ ] Track torrent file sizes and piece counts from bencode parsing
- [ ] Extract tracker lists from torrent files
- [ ] Compute info hashes from torrent files
- [ ] Track seed/peer counts over time

### Phase 4: Re-Sync & Updates

- [ ] Periodic re-sync to catch new movies
- [ ] Detect and update changed movie metadata
- [ ] Download new torrent variants as they appear
- [ ] Handle removed movies (soft delete)

### Phase 5: File Crawler & Matching

- [ ] Scan local movie files on disk
- [ ] Hash movie files for identification
- [ ] Match local files to YTS movies in the database
- [ ] Track which movies are available locally vs not

## API Strategy

1. **Primary**: YTS API at `https://yts.mx/api/v2/list_movies.json`
2. **Pagination**: 50 movies per page, ordered by date_added ascending
3. **Rate limiting**: 1000ms delay between requests
4. **Fallback**: If API is unavailable, page scraping from `https://www3.yts-official.to/`

## Storage Layout

```
Data/
└── oliver.db               # SQLite database

YtsData/
├── torrents/               # ~60,000+ torrent files
│   ├── {hash1}.torrent
│   └── ...
└── images/                 # ~30,000+ movie folders
    ├── {ytsId}/
    │   ├── small_cover.jpg
    │   ├── medium_cover.jpg
    │   ├── large_cover.jpg
    │   ├── background.jpg
    │   └── background_original.jpg
    └── ...
```
