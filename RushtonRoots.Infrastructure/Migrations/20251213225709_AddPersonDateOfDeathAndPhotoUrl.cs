using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushtonRoots.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonDateOfDeathAndPhotoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfDeath",
                table: "People",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "People",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfDeath",
                table: "People");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "People");
        }
    }
}
