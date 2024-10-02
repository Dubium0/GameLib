using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLibApi.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Games");

            migrationBuilder.AddColumn<string>(
                name: "BackgroundImageUrl",
                table: "Games",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MetaCritic",
                table: "Games",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundImageUrl",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "MetaCritic",
                table: "Games");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Games",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
