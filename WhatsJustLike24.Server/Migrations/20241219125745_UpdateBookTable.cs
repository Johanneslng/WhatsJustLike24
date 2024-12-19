using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsJustLike24.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Isbn",
                table: "BookDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Isbn13",
                table: "BookDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "BookDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Series",
                table: "BookDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Isbn",
                table: "BookDetails");

            migrationBuilder.DropColumn(
                name: "Isbn13",
                table: "BookDetails");

            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "BookDetails");

            migrationBuilder.DropColumn(
                name: "Series",
                table: "BookDetails");
        }
    }
}
