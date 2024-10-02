using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.GenreDtos;

public record class UpdateGenreDto(
    [Required] string Name
);