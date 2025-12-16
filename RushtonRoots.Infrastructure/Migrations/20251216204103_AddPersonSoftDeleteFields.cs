using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushtonRoots.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonSoftDeleteFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedDateTime",
                table: "People",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "People",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "People",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "People",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivedDateTime",
                table: "People");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "People");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "People");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "People");
        }
    }
}
