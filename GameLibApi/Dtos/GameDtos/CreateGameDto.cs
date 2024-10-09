using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.GameDtos;

public record class CreateGameDto(
    [Required] string Name,
    [Required] List<int> GenreIds,
    [Required] List<int> PlatformIds,
    [Required] DateOnly ReleaseDate,
    [Required] [Range(0,100)] int MetaCritic,
    [Url] string? BackgroundImageUrl
);

