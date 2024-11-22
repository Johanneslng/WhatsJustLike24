using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsJustLike24.Server.Migrations
{
    /// <inheritdoc />
    public partial class SimilarityTVF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE FUNCTION dbo.GetMovieSimilarityDetails (@Title NVARCHAR(MAX))
                RETURNS TABLE
                AS
                RETURN
                (
                    SELECT 
                        m2.Id,
                        m2.Title,
                        md2.PosterPath,
                        AVG(il.SimilarityScore) AS AverageSimilarityScore,
                        COUNT(il.SimilarityScore) AS SimilarityScoreCount,
                        STRING_AGG(il.description, '; ') WITHIN GROUP (ORDER BY il.description) AS DescriptionList,
                        STRING_AGG(il.SimilarityScore, '; ') WITHIN GROUP (ORDER BY il.description) AS SimilarityScoreList
                    FROM Movies m
                    JOIN MovieIsLike mil 
                        ON (m.Id = mil.MovieIdA OR m.Id = mil.MovieIdB) 
                        AND m.Title = @Title
                    JOIN Movies m2 
                        ON (m2.Id = mil.MovieIdA OR m2.Id = mil.MovieIdB) 
                        AND m2.Id != m.Id
                    LEFT JOIN MovieDetails md2 
                        ON m2.Id = md2.MovieId
                    LEFT JOIN IsLikeDetails il 
                        ON mil.Id = il.MovieIsLikeId
                    GROUP BY m2.Id, m2.Title, md2.PosterPath
                );
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS dbo.GetMovieSimilarityDetails;");
        }
    }
}
