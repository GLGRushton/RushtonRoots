using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushtonRoots.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddParentChildNotesAndConfidenceScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConfidenceScore",
                table: "ParentChildren",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ParentChildren",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfidenceScore",
                table: "ParentChildren");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ParentChildren");
        }
    }
}
