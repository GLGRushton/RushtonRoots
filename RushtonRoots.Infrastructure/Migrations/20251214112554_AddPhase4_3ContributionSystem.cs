using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushtonRoots.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhase4_3ContributionSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityFeedItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ActivityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ActionUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Points = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityFeedItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityFeedItems_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contributions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: false),
                    FieldName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OldValue = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    NewValue = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ContributorUserId = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    ReviewerUserId = table.Column<string>(type: "TEXT", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReviewNotes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CitationId = table.Column<int>(type: "INTEGER", nullable: true),
                    RequiresCitation = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contributions_AspNetUsers_ContributorUserId",
                        column: x => x.ContributorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contributions_AspNetUsers_ReviewerUserId",
                        column: x => x.ReviewerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contributions_Citations_CitationId",
                        column: x => x.CitationId,
                        principalTable: "Citations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ContributionScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    TotalPoints = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    ContributionsSubmitted = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    ContributionsApproved = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    ContributionsRejected = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CitationsAdded = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    ConflictsResolved = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    PeopleAdded = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    PhotosUploaded = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    StoriesWritten = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    LastActivityDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CurrentRank = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "Novice"),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributionScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContributionScores_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FactCitations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: false),
                    FieldName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CitationId = table.Column<int>(type: "INTEGER", nullable: false),
                    ConfidenceLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "Medium"),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    AddedByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactCitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactCitations_AspNetUsers_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FactCitations_Citations_CitationId",
                        column: x => x.CitationId,
                        principalTable: "Citations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConflictResolutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: false),
                    FieldName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ContributionId = table.Column<int>(type: "INTEGER", nullable: true),
                    ConflictType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CurrentValue = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ConflictingValue = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "Open"),
                    Resolution = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ResolutionNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ResolvedByUserId = table.Column<string>(type: "TEXT", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AcceptedCitationId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConflictResolutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConflictResolutions_AspNetUsers_ResolvedByUserId",
                        column: x => x.ResolvedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConflictResolutions_Citations_AcceptedCitationId",
                        column: x => x.AcceptedCitationId,
                        principalTable: "Citations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ConflictResolutions_Contributions_ContributionId",
                        column: x => x.ContributionId,
                        principalTable: "Contributions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContributionApprovals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContributionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ApproverUserId = table.Column<string>(type: "TEXT", nullable: false),
                    Decision = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    DecisionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsFinalDecision = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributionApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContributionApprovals_AspNetUsers_ApproverUserId",
                        column: x => x.ApproverUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContributionApprovals_Contributions_ContributionId",
                        column: x => x.ContributionId,
                        principalTable: "Contributions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityFeedItems_ActivityType",
                table: "ActivityFeedItems",
                column: "ActivityType");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityFeedItems_CreatedDateTime",
                table: "ActivityFeedItems",
                column: "CreatedDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityFeedItems_IsPublic",
                table: "ActivityFeedItems",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityFeedItems_UserId",
                table: "ActivityFeedItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConflictResolutions_AcceptedCitationId",
                table: "ConflictResolutions",
                column: "AcceptedCitationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConflictResolutions_ContributionId",
                table: "ConflictResolutions",
                column: "ContributionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConflictResolutions_EntityType_EntityId",
                table: "ConflictResolutions",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_ConflictResolutions_ResolvedByUserId",
                table: "ConflictResolutions",
                column: "ResolvedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConflictResolutions_Status",
                table: "ConflictResolutions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionApprovals_ApproverUserId",
                table: "ContributionApprovals",
                column: "ApproverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionApprovals_ContributionId",
                table: "ContributionApprovals",
                column: "ContributionId");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionApprovals_DecisionDate",
                table: "ContributionApprovals",
                column: "DecisionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_CitationId",
                table: "Contributions",
                column: "CitationId");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_ContributorUserId",
                table: "Contributions",
                column: "ContributorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_CreatedDateTime",
                table: "Contributions",
                column: "CreatedDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_EntityType_EntityId",
                table: "Contributions",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_ReviewerUserId",
                table: "Contributions",
                column: "ReviewerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_Status",
                table: "Contributions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionScores_LastActivityDate",
                table: "ContributionScores",
                column: "LastActivityDate");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionScores_TotalPoints",
                table: "ContributionScores",
                column: "TotalPoints");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionScores_UserId",
                table: "ContributionScores",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FactCitations_AddedByUserId",
                table: "FactCitations",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FactCitations_CitationId",
                table: "FactCitations",
                column: "CitationId");

            migrationBuilder.CreateIndex(
                name: "IX_FactCitations_EntityType_EntityId_FieldName",
                table: "FactCitations",
                columns: new[] { "EntityType", "EntityId", "FieldName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityFeedItems");

            migrationBuilder.DropTable(
                name: "ConflictResolutions");

            migrationBuilder.DropTable(
                name: "ContributionApprovals");

            migrationBuilder.DropTable(
                name: "ContributionScores");

            migrationBuilder.DropTable(
                name: "FactCitations");

            migrationBuilder.DropTable(
                name: "Contributions");
        }
    }
}
