using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.GenreDtos;

public record class CreateGenreDto(
    [Required] string Name
);