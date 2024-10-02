using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.PlatformDtos;

public record class CreatePlatformDto(
    [Required] string Name
);