using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.GameDtos;

public record class CreateGameDto(
    [Required] string Name,
    [Required] List<string> GenreNames,
    [Required] List<string> PlatformNames,
    [Required] DateOnly ReleaseDate,
    [Required] [Range(0,100)] int MetaCritic,
    [Url] string? BackgroundImageUrl
);

