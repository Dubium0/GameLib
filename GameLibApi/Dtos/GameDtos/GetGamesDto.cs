using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.GameDtos;

public record class GetGamesDto
(   // any string field should be comma seperated
    // adding 'r' to any filter will reverse the result ( for only metacritic and dates)
    [Range(1,double.MaxValue)] int? page,
    List<string> genres, // anyGenre,anyGenre,anyGenre .....
    List<string> platforms, // anyPlatform,anyPlatform,anyPlatform ...
    string? metacritic ,// LOW,HIGH format or only one parameter
    string? dates, // Year-month-day format or LOW,HIGH with same format
    [Range(1,200)] int pageSize = 5 //default page size is 5
);
