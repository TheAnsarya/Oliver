using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Oliver.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BasePath = table.Column<string>(type: "TEXT", nullable: true),
                    FilePath = table.Column<string>(type: "TEXT", nullable: true),
                    Size = table.Column<long>(type: "INTEGER", nullable: false),
                    MD5 = table.Column<string>(type: "TEXT", nullable: true),
                    SHA1 = table.Column<string>(type: "TEXT", nullable: true),
                    SHA256 = table.Column<string>(type: "TEXT", nullable: true),
                    LastHashAttempt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastVerifiedAttempt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Verified = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    YtsId = table.Column<int>(type: "INTEGER", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    ImdbCode = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    TitleEnglish = table.Column<string>(type: "TEXT", nullable: true),
                    TitleLong = table.Column<string>(type: "TEXT", nullable: true),
                    Slug = table.Column<string>(type: "TEXT", nullable: true),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Rating = table.Column<decimal>(type: "TEXT", nullable: false),
                    Runtime = table.Column<int>(type: "INTEGER", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", nullable: true),
                    DescriptionFull = table.Column<string>(type: "TEXT", nullable: true),
                    Synopsis = table.Column<string>(type: "TEXT", nullable: true),
                    YtTrailerCode = table.Column<string>(type: "TEXT", nullable: true),
                    Language = table.Column<string>(type: "TEXT", nullable: true),
                    MpaRating = table.Column<string>(type: "TEXT", nullable: true),
                    BackgroundImage = table.Column<string>(type: "TEXT", nullable: true),
                    BackgroundImageOriginal = table.Column<string>(type: "TEXT", nullable: true),
                    SmallCoverImage = table.Column<string>(type: "TEXT", nullable: true),
                    MediumCoverImage = table.Column<string>(type: "TEXT", nullable: true),
                    LargeCoverImage = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    DateUploaded = table.Column<string>(type: "TEXT", nullable: true),
                    DateUploadedUnix = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TorrentFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Hash = table.Column<string>(type: "TEXT", nullable: true),
                    Filename = table.Column<string>(type: "TEXT", nullable: true),
                    FilePath = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    PieceSize = table.Column<int>(type: "INTEGER", nullable: false),
                    Pieces = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MD5 = table.Column<string>(type: "TEXT", nullable: true),
                    SHA1 = table.Column<string>(type: "TEXT", nullable: true),
                    SHA256 = table.Column<string>(type: "TEXT", nullable: true),
                    IsMultiFile = table.Column<bool>(type: "INTEGER", nullable: false),
                    AnalyzedStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GenreStrings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    MovieId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreStrings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenreStrings_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TorrentDataFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Folder = table.Column<string>(type: "TEXT", nullable: true),
                    SubPath = table.Column<string>(type: "TEXT", nullable: true),
                    Filename = table.Column<string>(type: "TEXT", nullable: true),
                    Size = table.Column<long>(type: "INTEGER", nullable: false),
                    LastFindAttempt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastVerifiedAttempt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Verified = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataFileId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TorrentFileId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentDataFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TorrentDataFiles_DataFiles_DataFileId",
                        column: x => x.DataFileId,
                        principalTable: "DataFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TorrentDataFiles_TorrentFiles_TorrentFileId",
                        column: x => x.TorrentFileId,
                        principalTable: "TorrentFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TorrentInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    Hash = table.Column<string>(type: "TEXT", nullable: true),
                    Quality = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Size = table.Column<string>(type: "TEXT", nullable: true),
                    SizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    DateUploaded = table.Column<string>(type: "TEXT", nullable: true),
                    DateUploadedUnix = table.Column<long>(type: "INTEGER", nullable: false),
                    Current = table.Column<bool>(type: "INTEGER", nullable: false),
                    TorrentFileId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MovieId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TorrentInfos_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TorrentInfos_TorrentFiles_TorrentFileId",
                        column: x => x.TorrentFileId,
                        principalTable: "TorrentFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenreStrings_MovieId",
                table: "GenreStrings",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_TorrentDataFiles_DataFileId",
                table: "TorrentDataFiles",
                column: "DataFileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TorrentDataFiles_TorrentFileId",
                table: "TorrentDataFiles",
                column: "TorrentFileId");

            migrationBuilder.CreateIndex(
                name: "IX_TorrentInfos_MovieId",
                table: "TorrentInfos",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_TorrentInfos_TorrentFileId",
                table: "TorrentInfos",
                column: "TorrentFileId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenreStrings");

            migrationBuilder.DropTable(
                name: "TorrentDataFiles");

            migrationBuilder.DropTable(
                name: "TorrentInfos");

            migrationBuilder.DropTable(
                name: "DataFiles");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "TorrentFiles");
        }
    }
}
