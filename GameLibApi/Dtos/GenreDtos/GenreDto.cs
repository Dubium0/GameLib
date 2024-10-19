using System.ComponentModel.DataAnnotations;
namespace GameLibApi.Dtos.GenreDtos;


public record class GenreDto( 
    [Required] int Id,
    [Required] string Name
);
