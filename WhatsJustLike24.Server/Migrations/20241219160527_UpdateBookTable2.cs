using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsJustLike24.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Isbn13",
                table: "BookDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Isbn",
                table: "BookDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Languages",
                table: "BookDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pages",
                table: "BookDetails",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Languages",
                table: "BookDetails");

            migrationBuilder.DropColumn(
                name: "Pages",
                table: "BookDetails");

            migrationBuilder.AlterColumn<long>(
                name: "Isbn13",
                table: "BookDetails",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Isbn",
                table: "BookDetails",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
