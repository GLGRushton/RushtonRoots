using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushtonRoots.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoGalleryFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlobName",
                table: "PersonPhotos",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "PersonPhotos",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "PersonPhotos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "PhotoAlbumId",
                table: "PersonPhotos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "PersonPhotos",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PhotoAlbums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    AlbumDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CoverPhotoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoAlbums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoAlbums_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhotoTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PersonPhotoId = table.Column<int>(type: "INTEGER", nullable: false),
                    PersonId = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    XPosition = table.Column<int>(type: "INTEGER", nullable: true),
                    YPosition = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoTags_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhotoTags_PersonPhotos_PersonPhotoId",
                        column: x => x.PersonPhotoId,
                        principalTable: "PersonPhotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhotoPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PersonPhotoId = table.Column<int>(type: "INTEGER", nullable: true),
                    PhotoAlbumId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    HouseholdId = table.Column<int>(type: "INTEGER", nullable: true),
                    PermissionLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoPermissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotoPermissions_Households_HouseholdId",
                        column: x => x.HouseholdId,
                        principalTable: "Households",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotoPermissions_PersonPhotos_PersonPhotoId",
                        column: x => x.PersonPhotoId,
                        principalTable: "PersonPhotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotoPermissions_PhotoAlbums_PhotoAlbumId",
                        column: x => x.PhotoAlbumId,
                        principalTable: "PhotoAlbums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonPhotos_PhotoAlbumId",
                table: "PersonPhotos",
                column: "PhotoAlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoAlbums_CreatedByUserId",
                table: "PhotoAlbums",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoPermissions_HouseholdId",
                table: "PhotoPermissions",
                column: "HouseholdId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoPermissions_PersonPhotoId_HouseholdId",
                table: "PhotoPermissions",
                columns: new[] { "PersonPhotoId", "HouseholdId" });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoPermissions_PersonPhotoId_UserId",
                table: "PhotoPermissions",
                columns: new[] { "PersonPhotoId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoPermissions_PhotoAlbumId_HouseholdId",
                table: "PhotoPermissions",
                columns: new[] { "PhotoAlbumId", "HouseholdId" });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoPermissions_PhotoAlbumId_UserId",
                table: "PhotoPermissions",
                columns: new[] { "PhotoAlbumId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoPermissions_UserId",
                table: "PhotoPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoTags_PersonId",
                table: "PhotoTags",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoTags_PersonPhotoId",
                table: "PhotoTags",
                column: "PersonPhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonPhotos_PhotoAlbums_PhotoAlbumId",
                table: "PersonPhotos",
                column: "PhotoAlbumId",
                principalTable: "PhotoAlbums",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonPhotos_PhotoAlbums_PhotoAlbumId",
                table: "PersonPhotos");

            migrationBuilder.DropTable(
                name: "PhotoPermissions");

            migrationBuilder.DropTable(
                name: "PhotoTags");

            migrationBuilder.DropTable(
                name: "PhotoAlbums");

            migrationBuilder.DropIndex(
                name: "IX_PersonPhotos_PhotoAlbumId",
                table: "PersonPhotos");

            migrationBuilder.DropColumn(
                name: "BlobName",
                table: "PersonPhotos");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "PersonPhotos");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "PersonPhotos");

            migrationBuilder.DropColumn(
                name: "PhotoAlbumId",
                table: "PersonPhotos");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "PersonPhotos");
        }
    }
}
