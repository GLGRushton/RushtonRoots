using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushtonRoots.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeAndTraditionEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Ingredients = table.Column<string>(type: "TEXT", nullable: false),
                    Instructions = table.Column<string>(type: "TEXT", nullable: false),
                    PrepTimeMinutes = table.Column<int>(type: "INTEGER", nullable: true),
                    CookTimeMinutes = table.Column<int>(type: "INTEGER", nullable: true),
                    Servings = table.Column<int>(type: "INTEGER", nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Cuisine = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PhotoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    OriginatorPersonId = table.Column<int>(type: "INTEGER", nullable: true),
                    SubmittedByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    IsFavorite = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    AverageRating = table.Column<decimal>(type: "TEXT", precision: 3, scale: 2, nullable: false, defaultValue: 0m),
                    RatingCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipes_AspNetUsers_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recipes_People_OriginatorPersonId",
                        column: x => x.OriginatorPersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Traditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Frequency = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StartedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StartedByPersonId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "Active"),
                    PhotoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    HowToCelebrate = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    AssociatedItems = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    SubmittedByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Traditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Traditions_AspNetUsers_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Traditions_People_StartedByPersonId",
                        column: x => x.StartedByPersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RecipeRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecipeId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    HasMade = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeRatings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecipeRatings_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraditionTimelines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TraditionId = table.Column<int>(type: "INTEGER", nullable: false),
                    EventDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RecordedByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    PhotoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraditionTimelines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TraditionTimelines_AspNetUsers_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TraditionTimelines_Traditions_TraditionId",
                        column: x => x.TraditionId,
                        principalTable: "Traditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeRatings_RecipeId_UserId",
                table: "RecipeRatings",
                columns: new[] { "RecipeId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeRatings_UserId",
                table: "RecipeRatings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_OriginatorPersonId",
                table: "Recipes",
                column: "OriginatorPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Slug",
                table: "Recipes",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_SubmittedByUserId",
                table: "Recipes",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Traditions_Slug",
                table: "Traditions",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Traditions_StartedByPersonId",
                table: "Traditions",
                column: "StartedByPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Traditions_SubmittedByUserId",
                table: "Traditions",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TraditionTimelines_RecordedByUserId",
                table: "TraditionTimelines",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TraditionTimelines_TraditionId",
                table: "TraditionTimelines",
                column: "TraditionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeRatings");

            migrationBuilder.DropTable(
                name: "TraditionTimelines");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "Traditions");
        }
    }
}
