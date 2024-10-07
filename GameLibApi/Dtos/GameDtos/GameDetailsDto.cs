using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.GameDtos;

public record class GameDetailsDto(
    [Required] int Id,
    [Required] string Name,
    [Required] List<int> GenreId,
    [Required] List<int>  PlatformId,
    [Required] DateOnly ReleaseDate,
    [Required] int MetaCritic,
    string? BackgroundImageUrl
);

