using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsJustLike24.Server.Migrations
{
    /// <inheritdoc />
    public partial class SimilarityTVFGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SimilarityByTitleDTO",
                columns: table => new
                {
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AverageSimilarityScore = table.Column<int>(type: "int", nullable: true),
                    SimilarityScoreCount = table.Column<int>(type: "int", nullable: true),
                    PosterPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionList = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SimilarityScoreList = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.Sql(@"
                CREATE FUNCTION dbo.GetGameSimilarityDetails (@Title NVARCHAR(MAX))
                RETURNS TABLE
                AS
                RETURN
                (
                    SELECT 
                        g2.Id,
                        g2.Title,
                        gd2.Cover AS PosterPath,
                        AVG(il.SimilarityScore) AS AverageSimilarityScore,
                        COUNT(il.SimilarityScore) AS SimilarityScoreCount,
                        STRING_AGG(il.description, '; ') WITHIN GROUP (ORDER BY il.description) AS DescriptionList,
                        STRING_AGG(il.SimilarityScore, '; ') WITHIN GROUP (ORDER BY il.description) AS SimilarityScoreList
                    FROM Games g
                    JOIN GameIsLike gil 
                        ON (g.Id = gil.GameIdA OR g.Id = gil.GameIdB) 
                        AND g.Title = @Title
                    JOIN Games g2 
                        ON (g2.Id = gil.GameIdA OR g2.Id = gil.GameIdB) 
                        AND g2.Id != g.Id
                    LEFT JOIN GameDetails gd2 
                        ON g2.Id = gd2.GameId
                    LEFT JOIN GameIsLikeDetails il 
                        ON gil.Id = il.GameIsLikeId
                    GROUP BY g2.Id, g2.Title, gd2.Cover
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SimilarityByTitleDTO");

            migrationBuilder.Sql("DROP FUNCTION IF EXISTS dbo.GetGameSimilarityDetails;");
        }
    }
}
