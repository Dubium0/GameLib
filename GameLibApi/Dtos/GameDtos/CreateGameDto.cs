using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.GameDtos;

public record class CreateGameDto(
    [Required] string Name,
    [Required] int GenreId,
    [Required] int PlatformId,
    [Required] DateOnly ReleaseDate,
    [Required] int MetaCritic,
    string? BackgroundImageUrl
);

