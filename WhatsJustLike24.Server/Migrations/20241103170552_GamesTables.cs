using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsJustLike24.Server.Migrations
{
    /// <inheritdoc />
    public partial class GamesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstRelease = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Franchise = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Developer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Platforms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cover = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameDetails_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameIsLike",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameIdA = table.Column<int>(type: "int", nullable: false),
                    GameIdB = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameIsLike", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameIsLike_Games_GameIdA",
                        column: x => x.GameIdA,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameIsLike_Games_GameIdB",
                        column: x => x.GameIdB,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameIsLikeDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SimilarityScore = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameIsLikeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameIsLikeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameIsLikeDetails_GameIsLike_GameIsLikeId",
                        column: x => x.GameIsLikeId,
                        principalTable: "GameIsLike",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameDetails_GameId",
                table: "GameDetails",
                column: "GameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameIsLike_GameIdA",
                table: "GameIsLike",
                column: "GameIdA");

            migrationBuilder.CreateIndex(
                name: "IX_GameIsLike_GameIdB",
                table: "GameIsLike",
                column: "GameIdB");

            migrationBuilder.CreateIndex(
                name: "IX_GameIsLikeDetails_GameIsLikeId",
                table: "GameIsLikeDetails",
                column: "GameIsLikeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameDetails");

            migrationBuilder.DropTable(
                name: "GameIsLikeDetails");

            migrationBuilder.DropTable(
                name: "GameIsLike");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
