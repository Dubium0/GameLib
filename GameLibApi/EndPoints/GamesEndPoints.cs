using System;
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

        group.MapGet("/", async (GameLibContext dbContext) => 
        {
          
            IQueryable<Game> games = dbContext.Games.OrderBy(game => game.Id).Take(10);
            IQueryable<Genre> genres = dbContext.Genres;
            IQueryable<RGameGenre> rGameGenres = dbContext.RGameGenre;
            IQueryable<Platform> platforms = dbContext.Platforms;
            IQueryable<RGamePlatform> rGamePlatforms = dbContext.RGamePlatforms;

          
            var joinTable = from game in games
                            join rGameGenre in rGameGenres on game.Id equals rGameGenre.GameId
                            select new { Name = game.Name, GenreId = rGameGenre.GenreId };
             


            return Results.Ok( await joinTable.ToListAsync());
        }
        );

        group.MapGet("/{id}", async (int id, GameLibContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(id);

            if(game == null){
                return Results.NotFound();
            }

            return Results.Ok();
        }
        ).WithName(GetGameEndpointName);

        group.MapPost("/",async (CreateGameDto newGame, GameLibContext dbContext) => 
        {
            Game game = newGame.ToEntity(); 
           //foreach(var genreId in newGame.GenreIds){
           //    var existingGenre = await dbContext
           //                                .Genres
           //                                .FindAsync(genreId);
           //    if(existingGenre==null){
           //        // genre does not exist :<
           //        // send Genre not found 
           //        return Results.NotFound($"Genre {genreId} not found!");
           //    }
           //}

           //foreach(var platformId in newGame.PlatformIds){
           //    var existingPlatform = await dbContext
           //                                .Platforms
           //                                .FindAsync(platformId);
           //    if(existingPlatform==null){
           //        // genre does not exist :<
           //        // send Genre not found 
           //        return Results.NotFound($"Platform {platformId} not found!");
           //    }
           //}
            
            // I now the platform and genre exist I can safely add the game and its relations
            // for safety i will use transaction
            using (var transaction = await dbContext.Database.BeginTransactionAsync()){

                try{
                    System.Console.Out.WriteLine("In the Transaction!");
                    IQueryable<Genre> genres = dbContext.Genres.AsNoTracking();
                    IQueryable<Platform> platforms = dbContext.Platforms.AsNoTracking();

                    System.Console.Out.WriteLine($"In coming game {game.Name}");

                    if( dbContext.Games.Where(game_ => game_.Name == game.Name ).Count() > 0 ){
                        await transaction.RollbackAsync();
                        return Results.Conflict();
                    }

                    dbContext
                        .Games
                        .Add(game); // add game 
                    await dbContext
                        .SaveChangesAsync();

                    var genreJoin = from genre in genres
                                    join genreName in newGame.GenreNames on genre.Name equals genreName
                                    select new RGameGenre{GameId = game.Id, GenreId = genre.Id};
                    
                     System.Console.Out.WriteLine($"Genre join {genreJoin}");
                    
                    foreach( var joined in genreJoin){
                        dbContext
                            .RGameGenre
                            .Add(joined);
                    } 
                      System.Console.Out.WriteLine($"Genre join added");
                    var platformJoin = from platform in platforms
                                    join platformName in newGame.PlatformNames on platform.Name equals platformName
                                    select new RGamePlatform{GameId = game.Id, PlatformId = platform.Id};
                    
                      System.Console.Out.WriteLine($"Platform join {platformJoin}");
                    foreach( var joined in platformJoin){
                        dbContext
                            .RGamePlatforms
                            .Add(joined);
                    }
                    System.Console.Out.WriteLine($"Platform join added");

                    var genreJoinList = genreJoin.ToList();
                    System.Console.Out.WriteLine($"genre join list count{genreJoinList.Count} ");
                    List<int> genreIds = new List<int>();
                    for (int i = 0 ; i < genreJoinList.Count ; i++){
                        genreIds.Add(genreJoinList[i].GenreId);
                    }
                    System.Console.Out.WriteLine($"Genre IDs created");
                    var platformJoinList = platformJoin.ToList();
                    List<int> platformIds = new List<int>();
                    for (int i =0 ; i < platformJoinList.Count ; i++){
                        platformIds.Add(platformJoinList[i].PlatformId);
                    }
                     System.Console.Out.WriteLine($"Platform IDs created");
                    await dbContext
                            .SaveChangesAsync();
                     System.Console.Out.WriteLine($"Database saved");
                    await transaction
                            .CommitAsync();

                    System.Console.Out.WriteLine($"Commited transaction created");
                    return Results.CreatedAtRoute(GetGameEndpointName, 
                            new { id = game.Id }, game.ToDetailsDto(genreIds,platformIds));

                }catch (Exception e){

                    await transaction.RollbackAsync();
                     System.Console.Out.WriteLine($"Exception {e}");
                    
                    return Results.Problem($"An error: {e} occurred while creating the game.");

                }

            }
             
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
