using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLibApi.Migrations
{
    /// <inheritdoc />
    public partial class DunnoWhatisChangedmk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RGameGenre_Games_GameId",
                table: "RGameGenre");

            migrationBuilder.DropForeignKey(
                name: "FK_RGameGenre_Genres_GenreId",
                table: "RGameGenre");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RGameGenre",
                table: "RGameGenre");

            migrationBuilder.RenameTable(
                name: "RGameGenre",
                newName: "RGameGenres");

            migrationBuilder.RenameIndex(
                name: "IX_RGameGenre_GenreId",
                table: "RGameGenres",
                newName: "IX_RGameGenres_GenreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RGameGenres",
                table: "RGameGenres",
                columns: new[] { "GameId", "GenreId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RGameGenres_Games_GameId",
                table: "RGameGenres",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RGameGenres_Genres_GenreId",
                table: "RGameGenres",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RGameGenres_Games_GameId",
                table: "RGameGenres");

            migrationBuilder.DropForeignKey(
                name: "FK_RGameGenres_Genres_GenreId",
                table: "RGameGenres");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RGameGenres",
                table: "RGameGenres");

            migrationBuilder.RenameTable(
                name: "RGameGenres",
                newName: "RGameGenre");

            migrationBuilder.RenameIndex(
                name: "IX_RGameGenres_GenreId",
                table: "RGameGenre",
                newName: "IX_RGameGenre_GenreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RGameGenre",
                table: "RGameGenre",
                columns: new[] { "GameId", "GenreId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RGameGenre_Games_GameId",
                table: "RGameGenre",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RGameGenre_Genres_GenreId",
                table: "RGameGenre",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
