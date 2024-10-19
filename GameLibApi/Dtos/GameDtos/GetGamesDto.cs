using System.ComponentModel.DataAnnotations;

namespace GameLibApi.Dtos.GameDtos;

public record class GetGamesDto
(   
   int? PageNumber, 
   int? PageCount,
   string? Metacritic, // exmp : metacritic=80,100 or metacritic=90
   string? Dates, // exmp : dates=2020-12-12,2021-12-12 should be interval :<
   string? Orderings, // exmp : orderings=metacritic or orderings=-metacritic
   string? Genre, // exmp : genre=5,7,4 should be indices
   string? Platform // exmp : platform=5,7,4 should be indices
);
