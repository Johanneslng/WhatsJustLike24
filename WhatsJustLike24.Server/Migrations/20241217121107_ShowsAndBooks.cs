using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsJustLike24.Server.Migrations
{
    /// <inheritdoc />
    public partial class ShowsAndBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstRelease = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cover = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookDetails_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookIsLike",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookIdA = table.Column<int>(type: "int", nullable: false),
                    BookIdB = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookIsLike", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookIsLike_Books_BookIdA",
                        column: x => x.BookIdA,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookIsLike_Books_BookIdB",
                        column: x => x.BookIdB,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShowDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Director = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PosterPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShowId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShowDetails_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShowIsLike",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShowIdA = table.Column<int>(type: "int", nullable: false),
                    ShowIdB = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowIsLike", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShowIsLike_Shows_ShowIdA",
                        column: x => x.ShowIdA,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShowIsLike_Shows_ShowIdB",
                        column: x => x.ShowIdB,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookIsLikeDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SimilarityScore = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookIsLikeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookIsLikeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookIsLikeDetails_BookIsLike_BookIsLikeId",
                        column: x => x.BookIsLikeId,
                        principalTable: "BookIsLike",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShowIsLikeDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SimilarityScore = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShowIsLikeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowIsLikeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShowIsLikeDetails_ShowIsLike_ShowIsLikeId",
                        column: x => x.ShowIsLikeId,
                        principalTable: "ShowIsLike",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"
                CREATE FUNCTION dbo.GetBookSimilarityDetails (@Title NVARCHAR(MAX))
                RETURNS TABLE
                AS
                RETURN
                (
                    SELECT 
                        b2.Id,
                        b2.Title,
                        bd2.Cover AS PosterPath,
                        AVG(il.SimilarityScore) AS AverageSimilarityScore,
                        COUNT(il.SimilarityScore) AS SimilarityScoreCount,
                        STRING_AGG(il.description, '; ') WITHIN GROUP (ORDER BY il.description) AS DescriptionList,
                        STRING_AGG(il.SimilarityScore, '; ') WITHIN GROUP (ORDER BY il.description) AS SimilarityScoreList
                    FROM Books b
                    JOIN BookIsLike bil 
                        ON (b.Id = bil.BookIdA OR b.Id = bil.BookIdB) 
                        AND b.Title = @Title
                    JOIN Books b2 
                        ON (b2.Id = bil.BookIdA OR b2.Id = bil.BookIdB) 
                        AND b2.Id != b.Id
                    LEFT JOIN BookDetails bd2 
                        ON b2.Id = bd2.BookId
                    LEFT JOIN BookIsLikeDetails il 
                        ON bil.Id = il.BookIsLikeId
                    GROUP BY b2.Id, b2.Title, bd2.Cover
                );
            ");

            migrationBuilder.Sql(@"
                CREATE FUNCTION dbo.GetShowSimilarityDetails (@Title NVARCHAR(MAX))
                RETURNS TABLE
                AS
                RETURN
                (
                    SELECT 
                        s2.Id,
                        s2.Title,
                        sd2.PosterPath,
                        AVG(il.SimilarityScore) AS AverageSimilarityScore,
                        COUNT(il.SimilarityScore) AS SimilarityScoreCount,
                        STRING_AGG(il.description, '; ') WITHIN GROUP (ORDER BY il.description) AS DescriptionList,
                        STRING_AGG(il.SimilarityScore, '; ') WITHIN GROUP (ORDER BY il.description) AS SimilarityScoreList
                    FROM Shows s
                    JOIN ShowIsLike sil 
                        ON (s.Id = sil.ShowIdA OR s.Id = sil.ShowIdB) 
                        AND s.Title = @Title
                    JOIN Shows s2 
                        ON (s2.Id = sil.ShowIdA OR s2.Id = sil.ShowIdB) 
                        AND s2.Id != s.Id
                    LEFT JOIN ShowDetails sd2 
                        ON s2.Id = sd2.ShowId
                    LEFT JOIN ShowIsLikeDetails il 
                        ON sil.Id = il.ShowIsLikeId
                    GROUP BY s2.Id, s2.Title, sd2.PosterPath
                );
            ");

            migrationBuilder.CreateIndex(
                name: "IX_BookDetails_BookId",
                table: "BookDetails",
                column: "BookId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookIsLike_BookIdA",
                table: "BookIsLike",
                column: "BookIdA");

            migrationBuilder.CreateIndex(
                name: "IX_BookIsLike_BookIdB",
                table: "BookIsLike",
                column: "BookIdB");

            migrationBuilder.CreateIndex(
                name: "IX_BookIsLikeDetails_BookIsLikeId",
                table: "BookIsLikeDetails",
                column: "BookIsLikeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowDetails_ShowId",
                table: "ShowDetails",
                column: "ShowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShowIsLike_ShowIdA",
                table: "ShowIsLike",
                column: "ShowIdA");

            migrationBuilder.CreateIndex(
                name: "IX_ShowIsLike_ShowIdB",
                table: "ShowIsLike",
                column: "ShowIdB");

            migrationBuilder.CreateIndex(
                name: "IX_ShowIsLikeDetails_ShowIsLikeId",
                table: "ShowIsLikeDetails",
                column: "ShowIsLikeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookDetails");

            migrationBuilder.DropTable(
                name: "BookIsLikeDetails");

            migrationBuilder.DropTable(
                name: "ShowDetails");

            migrationBuilder.DropTable(
                name: "ShowIsLikeDetails");

            migrationBuilder.DropTable(
                name: "BookIsLike");

            migrationBuilder.DropTable(
                name: "ShowIsLike");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Shows");

            migrationBuilder.Sql("DROP FUNCTION IF EXISTS dbo.GetBookSimilarityDetails;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS dbo.GetShowSimilarityDetails;");
        }
    }
}
