# Oliver Architecture Overview

## System Design

Oliver is a **.NET 10 web application** that combines a background data pipeline with a REST API for the UI dashboard. It fetches, stores, and organizes the complete YTS movie dataset while serving data to the React-based frontend.

## Core Pipeline

```
YTS API (list_movies.json)
    │
    ▼
┌──────────────────────┐
│   YtsApiClient       │ ◄── Paginated HTTP requests (50/page)
│   (Services/)        │     Retry with exponential backoff
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│   YtsSyncWorker      │ ◄── Orchestrates the full sync
│   (BackgroundServices│     Parallel downloads (SemaphoreSlim)
│   /YtsSyncWorker.cs) │
└──┬─────────┬─────────┘
   │         │
   ▼         ▼
┌──────┐  ┌──────────────┐
│  DB  │  │DownloadService│
│SQLite│  │  (Services/) │
└──┬───┘  └──────┬───────┘
   │             │
   │      ┌──────┴──────┐
   │      ▼             ▼
   │  YtsData/      YtsData/
   │  torrents/     images/
   │
   ▼
┌──────────────────────┐
│   Minimal API        │ ◄── /api/stats, /api/movies, /api/genres,
│   (Program.cs)       │     /api/sync-status
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│   oliver-ui          │ ◄── Vite 7 + React 19 + TypeScript 5.9
│   (Dashboard)        │     TanStack React Query
└──────────────────────┘
```

## Components

### BackgroundServices/YtsSyncWorker.cs

The main orchestrator. Runs three phases sequentially:

1. **SyncAllMovies** — Paginates through the entire YTS catalog, upserting movies/genres/torrents into the database
2. **DownloadAllTorrents** — Downloads `.torrent` files for every torrent not yet downloaded
3. **DownloadAllImages** — Downloads cover and background images for every movie not yet downloaded

Uses `SyncState` table to track the last completed page for resume support.

### Services/YtsApiClient.cs

HTTP client for the YTS API. Uses named `HttpClient` ("yts") with 30s timeout and custom User-Agent. Single method `GetMoviesPageAsync` returns a tuple of movies and total count.

### Services/DownloadService.cs

File downloader for torrents and images. Downloads to structured folder layout:

- Torrents: `YtsData/torrents/{hash}.torrent`
- Images: `YtsData/images/{ytsId}/{name}.jpg`

Skips already-downloaded files.

### Data/OliverContext.cs

EF Core 10 DbContext with SQLite. Auto-sets `CreatedDate`/`UpdatedDate` timestamps. Defines indexes on `Movie.YtsId` (unique), `TorrentInfo.Hash`, and `SyncState.Key` (unique).

### Domain Models

| Entity | Key Fields |
|--------|------------|
| **Movie** | YtsId, Title, Year, Rating, Runtime, ImdbCode, image URLs, ImagesDownloaded |
| **Genre** | Name, MovieId (FK) |
| **TorrentInfo** | Hash, Url, Quality, Type, Size, TorrentFileDownloaded, TorrentFilePath |
| **SyncState** | Key, Value (for tracking sync progress) |
| **Entity** | Base class: Id (Guid), CreatedDate, UpdatedDate |

### Domain/YTS/YtsModels.cs

DTOs for deserializing YTS API JSON responses. Maps to `System.Text.Json` attributes.

## Configuration

All tunable via `appsettings.json`:

- **Urls** — Server listen URL (default `http://localhost:5000`)
- **Database:Path** — SQLite file location
- **Yts:ApiBaseUrl** — API endpoint
- **Yts:PageSize** — Movies per API request (default 50)
- **Yts:RequestDelayMs** — Delay between API calls (default 1000ms)
- **Downloads:BasePath** — Root download directory
- **Downloads:TorrentsFolder** — Torrent subfolder name
- **Downloads:ImagesFolder** — Image subfolder name
- **Downloads:MaxConcurrency** — Parallel download limit (default 5)
- **Downloads:MaxRetries** — Retry count for failed downloads (default 3)

## API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/stats` | GET | Movie, torrent, image counts |
| `/api/movies` | GET | Paginated movie list with search/genre/quality filters |
| `/api/genres` | GET | Genre list with counts |
| `/api/sync-status` | GET | Sync progress state |

Query parameters for `/api/movies`: `page`, `limit`, `search`, `genre`, `quality`.

## Technology Stack

- .NET 10 (Web SDK with minimal APIs)
- EF Core 10 + SQLite
- Serilog.AspNetCore (Console + File sinks)
- System.Text.Json
- BencodeNET 5 (for future torrent file parsing)
- Vite 7.3 + React 19 + TypeScript 5.9 (UI)
- TanStack React Query 5 (data fetching)
