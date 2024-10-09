using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLibApi.Migrations
{
    /// <inheritdoc />
    public partial class GenreAndPlaformNamesMAdeUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Platforms_Name",
                table: "Platforms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Name",
                table: "Genres",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Platforms_Name",
                table: "Platforms");

            migrationBuilder.DropIndex(
                name: "IX_Genres_Name",
                table: "Genres");
        }
    }
}
