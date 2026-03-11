# Oliver

**Oliver** is a .NET 9 application that downloads, organizes, and manages the complete YTS movie dataset. It fetches all movie metadata from the YTS API, downloads torrent files and cover images, and stores everything in a SQLite database with a well-organized folder structure.

## Features

- **Full YTS API Sync** — Paginates through all movies, downloading complete metadata
- **Torrent File Downloads** — Downloads every `.torrent` file for all quality/codec variants
- **Image Downloads** — Downloads cover images and background images for every movie
- **SQLite Database** — Stores all metadata with proper schema, genres, and torrent info
- **Resume Support** — Tracks sync progress, can pick up where it left off
- **Structured Storage** — Organized folder hierarchy for downloads

## Architecture

```
Oliver/
├── BackgroundServices/
│   └── YtsSyncWorker.cs        # Main orchestrator (paginate → save → download)
├── Data/
│   └── OliverContext.cs         # EF Core 9 + SQLite context
├── Domain/
│   ├── Entity.cs                # Base entity (Id, timestamps)
│   ├── Movie.cs                 # Movie metadata
│   ├── Genre.cs                 # Genre → Movie mapping
│   ├── TorrentInfo.cs           # Torrent variant info + download tracking
│   ├── SyncState.cs             # Key/value sync progress tracking
│   └── YTS/
│       └── YtsModels.cs         # API response DTOs
├── Services/
│   ├── YtsApiClient.cs          # HTTP client for YTS API
│   └── DownloadService.cs       # Torrent + image file downloader
├── Program.cs                   # Entry point, DI setup
├── appsettings.json             # Configuration
└── Oliver.csproj                # .NET 9 Worker Service
```

## Download Folder Structure

```
YtsData/
├── torrents/
│   ├── {hash1}.torrent
│   ├── {hash2}.torrent
│   └── ...
└── images/
    ├── {ytsId1}/
    │   ├── small_cover.jpg
    │   ├── medium_cover.jpg
    │   ├── large_cover.jpg
    │   ├── background.jpg
    │   └── background_original.jpg
    ├── {ytsId2}/
    │   └── ...
    └── ...
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Build

```powershell
dotnet build Oliver.sln
```

### Run

```powershell
dotnet run --project Oliver
```

The application will:

1. Create the SQLite database at `Data/oliver.db`
2. Paginate through all YTS movies via the API
3. Save all movie metadata, genres, and torrent info to the database
4. Download all `.torrent` files to `YtsData/torrents/`
5. Download all cover/background images to `YtsData/images/{id}/`

### Configuration

Edit [Oliver/appsettings.json](Oliver/appsettings.json):

```json
{
  "Database": {
    "Path": "Data/oliver.db"
  },
  "Yts": {
    "ApiBaseUrl": "https://yts.mx/api/v2/",
    "WebsiteBaseUrl": "https://www3.yts-official.to/",
    "PageSize": 50,
    "RequestDelayMs": 1000
  },
  "Downloads": {
    "BasePath": "YtsData",
    "TorrentsFolder": "torrents",
    "ImagesFolder": "images"
  }
}
```

## Database Schema

| Table | Description |
|-------|-------------|
| **Movies** | Full movie metadata (title, year, rating, IMDb code, image URLs, etc.) |
| **Genres** | Genre names linked to movies (many-to-one) |
| **TorrentInfos** | Torrent variants per movie (hash, quality, type, size, download status) |
| **SyncStates** | Key/value store for tracking sync progress |

## Tech Stack

- **.NET 9** Worker Service
- **EF Core 9** with SQLite
- **Serilog** for structured logging
- **System.Text.Json** for API deserialization
- **BencodeNET 5** for future torrent parsing

## Related Projects

- [YTSOrganizer](https://github.com/TheAnsarya/YTSOrganizer) — Core data pipeline: API client, SQL Server storage, sync services
- [DownloadYTS](https://github.com/TheAnsarya/DownloadYTS) — Bulk downloader for YTS pages/torrents

## Documentation

- [Architecture Overview](~docs/architecture/overview.md)
- [Download Pipeline Plan](~docs/plans/download-pipeline-plan.md)
- [Session Logs](~docs/session-logs/)
- [Copilot Instructions](.github/copilot-instructions.md)
