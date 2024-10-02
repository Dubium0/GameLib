using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.PlatformDtos;

public record class PlatformDto( 
    [Required] int Id,
    [Required] string Name
);
