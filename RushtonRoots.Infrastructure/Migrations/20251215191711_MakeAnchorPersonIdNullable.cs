using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushtonRoots.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeAnchorPersonIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Households_AnchorPersonId",
                table: "Households");

            migrationBuilder.AlterColumn<int>(
                name: "AnchorPersonId",
                table: "Households",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Households_AnchorPersonId",
                table: "Households",
                column: "AnchorPersonId",
                unique: true,
                filter: "[AnchorPersonId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Households_AnchorPersonId",
                table: "Households");

            migrationBuilder.AlterColumn<int>(
                name: "AnchorPersonId",
                table: "Households",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Households_AnchorPersonId",
                table: "Households",
                column: "AnchorPersonId",
                unique: true);
        }
    }
}
