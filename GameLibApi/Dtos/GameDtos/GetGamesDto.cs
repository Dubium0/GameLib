using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.GameDtos;

public record class GetGamesDto
(
    [Range(1,double.MaxValue)] int? page,
    [Range(1,200)] int pageSize = 5,
    bool descendingOrder = false
);
