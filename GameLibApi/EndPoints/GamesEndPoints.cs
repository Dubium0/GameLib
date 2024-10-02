using GameLibApi.Data;
using GameLibApi.Dtos.GameDtos;
using GameLibApi.Entities;
using GameLibApi.Mappings;
using Microsoft.EntityFrameworkCore;
namespace GameLibApi.EndPoints;

public static class GamesEndPoints
{
    const string GetGameEndpointName = "GetGame";
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app){
        var group = app.MapGroup("api/games").WithParameterValidation();

        group.MapGet("/", async (GameLibContext dbContext,[AsParameters] GetGamesDto request) => 
        {
          
            IQueryable<Game> games = dbContext.Games.OrderBy(game => game.Id);

            if(request.descendingOrder){
                
                games = games.Where(game=> game.Id <= (request.page == null ? request.pageSize : request.page) )
                                                .OrderByDescending(game=> game.Id)
                                                .Take(request.pageSize)
                                                .OrderBy(game=> game.Id);
            }else{

                games = games.Where(game=> game.Id >= (request.page == null ? 1 : request.page))
                                    .Take(request.pageSize);
            }
            

            var pagedGames =  await games
                .Include(game => game.Genre)
                .Include(game => game.Platform)
                .Select(game => game.ToSummaryDto())
                .AsNoTracking()
                .ToListAsync();

            return Results.Ok(pagedGames);
        }
        );

        group.MapGet("/{id}", async (int id, GameLibContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(id);

            if(game == null){
                return Results.NotFound();
            }

            return Results.Ok(game.ToDetailsDto());
        }
        ).WithName(GetGameEndpointName);

        group.MapPost("/",async (CreateGameDto newGame, GameLibContext dbContext) => 
        {
            Game game =  newGame.ToEntity();
           
            dbContext
                .Games
                .Add(game);
                
            await dbContext
                .SaveChangesAsync();

            return Results.CreatedAtRoute(GetGameEndpointName, 
            new { id = game.Id}, game.ToDetailsDto());
        }
        );

        group.MapPut("/{id}",async (int id,UpdateGameDto updatedGame, GameLibContext dbContext) => 
        {
            var existingGame = await dbContext
                                        .Games
                                        .FindAsync(id);

            if(existingGame ==null){
                return Results.NotFound();
            }

            dbContext.Entry(existingGame)
                .CurrentValues
                .SetValues(updatedGame.ToEntity(id));

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        }
        );

        group.MapDelete("/{id}", async (int id, GameLibContext dbContext) =>{ 
            // batch delete
            await dbContext.Games
                .Where(game => game.Id == id)
                .ExecuteDeleteAsync();
            return Results.NoContent();
        }
        );
        return group;
    }
}
