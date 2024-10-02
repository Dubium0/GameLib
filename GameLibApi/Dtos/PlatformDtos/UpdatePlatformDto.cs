using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.PlatformDtos;

public record class UpdatePlatformDto(
    [Required] string Name
);
