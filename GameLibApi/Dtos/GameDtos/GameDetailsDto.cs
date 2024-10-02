using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.GameDtos;

public record class GameDetailsDto(
    [Required] int Id,
    [Required] string Name,
    [Required] int GenreId,
    [Required] int PlatformId,
    [Required] DateOnly ReleaseDate,
    [Required] int MetaCritic,
    string? BackgroundImageUrl
);

