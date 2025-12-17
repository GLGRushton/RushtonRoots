using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushtonRoots.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatemigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "ParentChildren",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisputeReason",
                table: "ParentChildren",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DisputedDateTime",
                table: "ParentChildren",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ParentChildren",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisputed",
                table: "ParentChildren",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "ParentChildren");

            migrationBuilder.DropColumn(
                name: "DisputeReason",
                table: "ParentChildren");

            migrationBuilder.DropColumn(
                name: "DisputedDateTime",
                table: "ParentChildren");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ParentChildren");

            migrationBuilder.DropColumn(
                name: "IsDisputed",
                table: "ParentChildren");
        }
    }
}
