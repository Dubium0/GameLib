using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.GameDtos;

public record class UpdateGameDto( 
    [Required] string Name,
    List<int>? GenreIds,
    List<int>? PlatformIds,
    [Required] DateOnly ReleaseDate,
    [Required] int MetaCritic,
    [Required] string BackgroundImageUrl
);
