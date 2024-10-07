using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.GameDtos;

public record class GameSummaryDto(
    [Required] int Id,
    [Required] string Name,
    [Required] List<string> Genre,
    [Required] List<string> Platform,
    [Required] DateOnly ReleaseDate,
    [Required] int MetaCritic,
    string? BackgroundImageUrl
);

