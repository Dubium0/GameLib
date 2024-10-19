


using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos;


public record PageDto<T>(
    [Required] int Count,
    [Required] bool HasPreviousPage,
    [Required] bool HasNextPage,
    [Required] List<T> Results // send empty
);