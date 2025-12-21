using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushtonRoots.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Traditions_Category",
                table: "Traditions",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Traditions_IsPublished",
                table: "Traditions",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Traditions_IsPublished_Status",
                table: "Traditions",
                columns: new[] { "IsPublished", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Traditions_Status",
                table: "Traditions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_Category",
                table: "Stories",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_IsPublished",
                table: "Stories",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_IsPublished_CreatedDateTime",
                table: "Stories",
                columns: new[] { "IsPublished", "CreatedDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Category",
                table: "Recipes",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_IsFavorite",
                table: "Recipes",
                column: "IsFavorite");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_IsPublished",
                table: "Recipes",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_IsPublished_AverageRating",
                table: "Recipes",
                columns: new[] { "IsPublished", "AverageRating" });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoAlbums_DisplayOrder_CreatedDateTime",
                table: "PhotoAlbums",
                columns: new[] { "DisplayOrder", "CreatedDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoAlbums_IsPublic",
                table: "PhotoAlbums",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_People_DateOfBirth",
                table: "People",
                column: "DateOfBirth");

            migrationBuilder.CreateIndex(
                name: "IX_People_IsDeceased",
                table: "People",
                column: "IsDeceased");

            migrationBuilder.CreateIndex(
                name: "IX_People_LastName",
                table: "People",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_People_LastName_FirstName",
                table: "People",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_ParentChildren_IsVerified",
                table: "ParentChildren",
                column: "IsVerified");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Traditions_Category",
                table: "Traditions");

            migrationBuilder.DropIndex(
                name: "IX_Traditions_IsPublished",
                table: "Traditions");

            migrationBuilder.DropIndex(
                name: "IX_Traditions_IsPublished_Status",
                table: "Traditions");

            migrationBuilder.DropIndex(
                name: "IX_Traditions_Status",
                table: "Traditions");

            migrationBuilder.DropIndex(
                name: "IX_Stories_Category",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Stories_IsPublished",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Stories_IsPublished_CreatedDateTime",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_Category",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_IsFavorite",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_IsPublished",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_IsPublished_AverageRating",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_PhotoAlbums_DisplayOrder_CreatedDateTime",
                table: "PhotoAlbums");

            migrationBuilder.DropIndex(
                name: "IX_PhotoAlbums_IsPublic",
                table: "PhotoAlbums");

            migrationBuilder.DropIndex(
                name: "IX_People_DateOfBirth",
                table: "People");

            migrationBuilder.DropIndex(
                name: "IX_People_IsDeceased",
                table: "People");

            migrationBuilder.DropIndex(
                name: "IX_People_LastName",
                table: "People");

            migrationBuilder.DropIndex(
                name: "IX_People_LastName_FirstName",
                table: "People");

            migrationBuilder.DropIndex(
                name: "IX_ParentChildren_IsVerified",
                table: "ParentChildren");
        }
    }
}
