# Oliver - AI Copilot Directives

## Project Overview

**Oliver** is a .NET 10 application that downloads, organizes, and manages the complete YTS movie dataset. It fetches all movie metadata from the YTS API, downloads torrent files and cover images, and stores everything in a SQLite database with a well-organized folder structure.

Key features and focus areas:

1. **YTS API Integration** — Full API client for movie list/details with pagination
2. **Database Storage** — SQLite with EF Core, comprehensive movie/genre/torrent schema
3. **Torrent File Downloading** — Download all .torrent files for every movie variant
4. **Image Downloading** — Download all cover images and background images
5. **File Organization** — Store downloads in a structured folder hierarchy
6. **Data Completeness** — Build a complete dataset for every YTS movie

## Related Projects

- **YTSOrganizer** — Core data pipeline: API client, SQL Server storage, sync services
- **DownloadYTS** — Bulk downloader for YTS pages/torrents when the API is insufficient

## GitHub Issue Management

### ⚠️ CRITICAL: Always Create Issues on GitHub Directly

**NEVER just document issues in markdown files.** Always create actual GitHub issues using the `gh` CLI:

```powershell
# Create an issue
gh issue create --repo TheAnsarya/Oliver --title "[X.Y] Issue Title" --body "Description" --label "label1,label2"

# Add labels
gh issue edit <number> --repo TheAnsarya/Oliver --add-label "label"

# Close issue
gh issue close <number> --repo TheAnsarya/Oliver --comment "Completed in commit abc123"
```

### Issue Numbering Convention

- Epic issues: `[Epic N]` (e.g., `[Epic 1] YTS API Data Pipeline`)
- Sub-issues: `[N.X]` (e.g., `[1.3] Implement Genre Sync Logic`)

### Required Labels

- `api` — API client related
- `database` — Database/EF Core related
- `sync` — Data synchronization
- `download` — Download pipeline
- `torrent` — Torrent file handling
- `images` — Image downloading
- `ui` — Future UI related
- `performance` — Performance related
- `testing` — Tests and benchmarks
- Priority: `high-priority`, `medium-priority`, `low-priority`
- `docs` — Documentation
- `epic` — Epic tracking issues
- `infra` — Infrastructure/tooling

### ⚠️ MANDATORY: Issue-First Workflow

**Always create GitHub issues BEFORE starting implementation work.** This is non-negotiable.

1. **Before Implementation:**
   - Create a GitHub issue describing the planned work
   - Include scope, approach, and acceptance criteria
   - Add appropriate labels

2. **During Implementation:**
   - Reference issue number in commits: `git commit -m "Fix movie sync pagination - #12"`
   - Update issue with progress comments if work spans multiple sessions
   - Add sub-issues for discovered work

3. **After Implementation:**
   - Close issue with completion comment including commit hash
   - Link related issues if applicable

**Workflow Pattern:**

```powershell
# 1. Create issue FIRST
gh issue create --repo TheAnsarya/Oliver --title "Description" --body "Details" --label "label"

# 2. Add prompt tracking comment (for AI-created issues)
gh issue comment <number> --repo TheAnsarya/Oliver --body "Prompt for work:`n{original user prompt}"

# 3. Implement the fix/feature

# 4. Commit with issue reference
git add .
git commit -m "Brief description - #<issue-number>"

# 5. Close issue with summary
gh issue close <number> --repo TheAnsarya/Oliver --comment "Completed in <commit-hash>"
```

### ⚠️ MANDATORY: Prompt Tracking for AI-Created Issues

When creating GitHub issues from AI prompts, **IMMEDIATELY** add the original user prompt as the **FIRST comment** right after creating the issue — before doing any implementation work:

```powershell
# Create issue
$issueUrl = gh issue create --repo TheAnsarya/Oliver --title "Description" --body "Details" --label "label"
$issueNum = ($issueUrl -split '/')[-1]

# IMMEDIATELY add prompt as first comment (before any other work)
gh issue comment $issueNum --repo TheAnsarya/Oliver --body "Prompt for work:
<original user prompt that triggered this work>"
```

**For sub-issues created during analysis/audit:**

```powershell
gh issue comment <number> --repo TheAnsarya/Oliver --body "Prompt for work:
Parent: #<parent-issue-number>
Original prompt: <the original user prompt that started the work>

Created during: <description of analysis/audit work>"
```

## Coding Standards

### Indentation

- **TABS for indentation** — enforced by `.editorconfig`
- **Tab width: 4 spaces** — ALWAYS use 4-space-equivalent tabs
- NEVER use spaces for indentation — only tabs

### Brace Style — K&R (One True Brace)

- **Opening braces on the SAME LINE** as the statement — ALWAYS
- This applies to ALL constructs: `if`, `else`, `for`, `while`, `switch`, `try`, `catch`, functions, methods, classes, structs, namespaces, lambdas, properties, enum declarations, etc.
- `else` and `else if` go on the same line as the closing brace: `} else {`
- `catch` goes on the same line as the closing brace: `} catch (...) {`
- **NEVER use Allman style** (brace on its own line)
- **NEVER put an opening brace on a new line** — not even for long parameter lists

#### C# Examples

```csharp
// ✅ CORRECT — K&R style
if (condition) {
	DoSomething();
} else {
	DoFallback();
}

public void Execute(int param) {
	// body
}

public class MyClass : Base {
	public string Name { get; set; }

	public void Method() {
		// body
	}
}

// ❌ WRONG — Allman style (DO NOT USE)
if (condition)
{
	DoSomething();
}
```

### C# Standard

- **.NET 10** with latest C# features (C# 14)
- File-scoped namespaces where applicable
- Nullable reference types enabled
- Implicit usings enabled

### Hexadecimal Values

- **Always lowercase**: `0xff00`, not `0xFF00`

### ⚠️ Comment Safety Rule

**When adding or modifying comments, NEVER change the actual code.**

- Changes to comments must not alter code logic, structure, or formatting
- When adding XML documentation or inline comments, preserve all existing code exactly
- Verify code integrity after adding documentation

## YTS Website & API

### Website URLs

- **Current website**: `https://www3.yts-official.to/` (also accessible via `https://www.yts-official.top/`)
- **Old website** (no longer active): `yts.mx`
- **API base**: `https://yts.mx/api/v2/` — try this first, fall back to page scraping if unavailable

### API Usage Strategy

1. **Prefer the API** for structured data (movie lists, details, torrents)
2. **Fall back to page scraping** when API doesn't provide all needed data
3. **Download ALL variations** of every movie (all qualities, codecs, etc.)
4. **Store raw responses** for audit and re-processing

### Data Completeness Goal

Every movie in the database must have:

- Full metadata (title, year, rating, runtime, IMDb code, descriptions, images)
- All genres mapped
- All torrent variants (720p, 1080p, 2160p; x264, x265; etc.)
- Torrent file downloads stored on disk
- Cover images stored on disk
- Info hashes computed and stored

## Performance Guidelines

### Testing Before Committing

1. Build succeeds: `dotnet build`
2. No new warnings in build output

### Database Performance

- Use EF Core efficiently — batch operations where possible
- Use transactions for multi-table operations
- Monitor query performance for large datasets

## Branch Strategy

- `main` — Stable release branch
- Feature branches for significant work

## Build Commands

```powershell
# Build
dotnet build Oliver.sln

# Run
dotnet run --project Oliver

# Clean build
dotnet clean Oliver.sln
dotnet build Oliver.sln
```

## Documentation

- Session logs: `~docs/session-logs/YYYY-MM-DD-session-NN.md`
- Plans: `~docs/plans/`
- Architecture docs: `~docs/architecture/`

## Problem-Solving Philosophy

### ⚠️ NEVER GIVE UP on Hard Problems

When a task is complex or seems difficult:

1. **NEVER declare something "too hard" or "not worth it"** and close the issue
2. **Break it down** — Create multiple smaller sub-issues for research, prototyping, and incremental progress
3. **Research first** — Create research issues to investigate approaches, alternatives, and prior art
4. **Document everything** — Create docs, code-plans, and analysis documents in `~docs/plans/`
5. **Prototype** — Create spike/prototype branches to test approaches before committing
6. **Incremental progress** — Even partial progress is valuable
7. **Create issues for future work** — If something can't be done now, create well-documented issues

### What "Closing Too Soon" Looks Like

- ❌ "This is deeply integrated, keeping as-is" → Instead: break it into phases
- ❌ "Migration cost-prohibitive" → Instead: create research issues and prototype
- ❌ "High regression risk" → Instead: create test plan and incremental migration
- ✅ Close only when the work is **actually complete** or **truly impossible** (not just hard)

## Markdown Formatting

### ⚠️ MANDATORY: Fix Markdownlint Warnings

**Always fix markdownlint warnings when editing or creating markdown files.** This is non-negotiable.

Key rules to enforce:

- **MD022** — Blank lines above and below headings
- **MD031** — Blank lines around fenced code blocks
- **MD032** — Blank lines around lists (ordered and unordered)
- **MD047** — Files must end with a single newline character
- **MD010** — Disabled (hard tabs are REQUIRED per our indentation rules)

### ⚠️ MANDATORY: Documentation Link-Tree

**Every markdown file in the repository must be reachable from `README.md` through a hierarchical link structure.**

- The main `README.md` must link to all documentation directories and key files
- Subdirectory docs should link back to parent and to sibling docs
- No orphan markdown files — if a `.md` file exists, it must be discoverable from the root README
- When adding new documentation, always update `README.md` with a link to it
