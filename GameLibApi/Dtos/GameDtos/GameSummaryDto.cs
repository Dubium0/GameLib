using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.GameDtos;

public record class GameSummaryDto(
    [Required] int Id,
    [Required] string Name,
    [Required] string Genre,
    [Required] string Platform,
    [Required] DateOnly ReleaseDate,
    [Required] int MetaCritic,
    string? BackgroundImageUrl
);

