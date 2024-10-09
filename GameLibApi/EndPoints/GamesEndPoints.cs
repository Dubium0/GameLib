using System.Diagnostics;
using GameLibApi.Data;
using GameLibApi.Dtos.GameDtos;
using GameLibApi.Entities;
using GameLibApi.Mappings;
using Microsoft.EntityFrameworkCore;
namespace GameLibApi.EndPoints;

public static class GamesEndPoints
{
    public const string GetGameEndpointName = "GetGame";
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app){
        var group = app.MapGroup("api/games").WithParameterValidation();

        group.MapGet("/", GetGamesWithSummary);

        group.MapGet("/{id}", GetGameById ).WithName(GetGameEndpointName);

        group.MapPost("/", PostGame );

        group.MapPut("/{id}",UpdateGame );

        group.MapDelete("/{id}", DeleteGame );

        return group;
    }

    public static async Task<IResult> GetGamesWithDetails(GameLibContext dbContext){

        IQueryable<Game> games = dbContext.Games.OrderBy(game => game.Id).Take(10);
        IQueryable<RGameGenre> rGameGenres = dbContext.RGameGenres;
        IQueryable<RGamePlatform> rGamePlatforms = dbContext.RGamePlatforms;
       
        return Results.Ok( await
            games.AsNoTracking()
                .Select( 
                    game => game.ToDetailsDto(
                        rGameGenres
                        .Where( 
                            rGameGenre => rGameGenre.GameId == game.Id
                        )
                        .Select( 
                            rGameGenre => rGameGenre.GenreId
                        )
                        .ToList()
                        ,
                        rGamePlatforms
                        .Where(
                            rGamePlatform => rGamePlatform.GameId == game.Id
                        )
                        .Select(
                            rGamePlatform => rGamePlatform.PlatformId
                        )
                        .ToList()
                    )
            )
            .ToListAsync()
        );
             
    }

    public static async Task<IResult> GetGamesWithSummary(GameLibContext dbContext){

        IQueryable<Game> games = dbContext.Games.OrderBy(game => game.Id).Take(10);
        IQueryable<RGameGenre> rGameGenres = dbContext.RGameGenres;
        IQueryable<RGamePlatform> rGamePlatforms = dbContext.RGamePlatforms;

         return Results.Ok( await
            games.AsNoTracking()
                .Select( 
                    game => game.ToSummaryDto(
                        
                        rGameGenres
                        .AsNoTracking()
                        .Where( 
                            rGameGenre => rGameGenre.GameId == game.Id
                        )
                        .Select(
                           rGameGenre => rGameGenre.Genre!.Name
                        )
                        .ToList()
                        ,
                        rGamePlatforms
                        .Where(
                            rGamePlatform => rGamePlatform.GameId == game.Id
                        )
                        .Select(
                            rGamePlatform => rGamePlatform.Platform!.Name
                        )
                        .ToList()
                    )
            )
            .ToListAsync()
        );



    }

    public static async Task<IResult> GetGameById(int id,GameLibContext dbContext){
        Game? game =await  dbContext.Games.FindAsync(id);

        if(game == null){
            return Results.NotFound();

        }

       

        var genreList = await dbContext
                            .RGameGenres
                            .AsNoTracking() 
                            .Where( rGameGenre => rGameGenre.GameId == game.Id)
                            .Select( rGameGenre => rGameGenre.GenreId )
                            .ToListAsync();


        var platformList = await dbContext
                            .RGamePlatforms
                            .AsNoTracking() 
                            .Where( rGamePlatform => rGamePlatform.GameId == game.Id)
                            .Select( rGamePlatform => rGamePlatform.PlatformId )
                            .ToListAsync();


        return Results.Ok(new GameDetailsDto(
                        game.Id,
                        game.Name,
                        genreList,
                        platformList,
                        game.ReleaseDate,
                        game.MetaCritic,
                        game.BackgroundImageUrl   
                    ));
    }

    public static async Task<IResult> PostGame(CreateGameDto newGame, GameLibContext dbContext){

        Game? game = newGame.ToEntity();
        
        if( await dbContext
                    .Games
                    .AsNoTracking()
                    .Select(game => game.Name)
                    .Where(gameName=> gameName == game.Name)
                    .AnyAsync() == true){
            
            return Results.Conflict($"Game name: {game.Name} exists in the database so I will not add it again");
        }


        var genreIds = await dbContext
                                .Genres
                                .AsNoTracking()
                                .Select(genre => genre.Id)
                                .Where(genreId => newGame.GenreIds.Contains(genreId))
                                .ToListAsync();

        if(genreIds == null){
            return Results.BadRequest("There is no existing genre ids corresponding to provided genre list ");
        }

        

        var platformIds = await dbContext
                                    .Platforms
                                    .AsNoTracking()
                                    .Select(platform => platform.Id)
                                    .Where(platformId => newGame.PlatformIds.Contains(platformId))
                                    .ToListAsync();

        if(platformIds == null){
            return Results.BadRequest("There is no existing platform ids corresponding to provided platform list ");
        }

        using (var transaction = await dbContext.Database.BeginTransactionAsync()){
            try{

            //first of all add the game into database
            dbContext
                .Games
                .Add(game); // add game 
            
            await dbContext
                    .SaveChangesAsync();

            List<RGameGenre> rGameGenresToAdd = new List<RGameGenre>(); 
            foreach(var genreId in genreIds){
                rGameGenresToAdd.Add(new RGameGenre{GameId = game.Id, GenreId = genreId});
            }

            await dbContext
                    .RGameGenres
                    .AddRangeAsync(rGameGenresToAdd);

            List<RGamePlatform> rGamePlatformsToAdd = new List<RGamePlatform>(); 
            foreach(var platformId  in platformIds){
                rGamePlatformsToAdd.Add(new RGamePlatform{GameId = game.Id, PlatformId = platformId});
            }
            await dbContext
                    .RGamePlatforms
                    .AddRangeAsync(rGamePlatformsToAdd);

            await dbContext
                    .SaveChangesAsync();
            
            await transaction
                    .CommitAsync();

       
            return Results.CreatedAtRoute(  GetGameEndpointName, 
                                            new { id = game.Id },
                                            game.ToDetailsDto(genreIds, platformIds)
                                         );
            }catch(Exception e){

                await transaction.RollbackAsync();
                
                Debug.WriteLine($"Exception: {e} occured and transaction rolled back ");

                return Results.Problem($"Problem occured during transaction!\nError: {e}");
                
            }

        }
    }

    public static async Task<IResult> UpdateGame(int id,UpdateGameDto updatedGame, GameLibContext dbContext){
        // make genre list and platform list optional to gain a little performance boost
        
        Game? existingGame = await dbContext.Games.FindAsync(id);

        if(existingGame == null){
            return Results.NotFound($"Game with id : {id} does not exists in database");
        }
        using (var transaction = await dbContext.Database.BeginTransactionAsync()){
            
            try{
                dbContext
                    .Entry(existingGame)
                    .CurrentValues
                    .SetValues(updatedGame.ToEntity(id));

                await dbContext.SaveChangesAsync();
                
                if(updatedGame.GenreIds !=null){

                    var existingRGameGenres = dbContext.RGameGenres
                                                        .Where(rGameGenre => rGameGenre.GameId == id);

                    var newRGameGenres = updatedGame.GenreIds
                                                .Select(genreId => new RGameGenre{GameId =id, GenreId = genreId});
                    dbContext.RGameGenres
                                .RemoveRange(existingRGameGenres);
                    
                    await dbContext.SaveChangesAsync();
                    
                    await dbContext.RGameGenres
                                .AddRangeAsync(newRGameGenres);
                    await dbContext.SaveChangesAsync();
                }

                if(updatedGame.PlatformIds !=null){

                    var existingRGamePlatforms = dbContext.RGamePlatforms
                                                        .Where(rGamePlatforms => rGamePlatforms.GameId == id);
                
                    var newRGamePlatforms = updatedGame.PlatformIds
                                                .Select(platformId => new RGamePlatform{GameId = id, PlatformId = platformId});
                    dbContext.RGamePlatforms
                            .RemoveRange(existingRGamePlatforms);

                    await dbContext.SaveChangesAsync();

                    await dbContext.RGamePlatforms.AddRangeAsync(newRGamePlatforms);

                    await dbContext.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return Results.NoContent();

            }catch(Exception e){
                await transaction.RollbackAsync();
                Debug.WriteLine($"Exception: {e} occured and transaction rolled back ");
                return Results.Problem($"Problem occured during transaction!\nError: {e}");
            }
        }
    }

    public static async Task<IResult> DeleteGame(int id, GameLibContext dbContext){
        // batch delete
        await dbContext.Games
            .Where(game => game.Id == id)
            .ExecuteDeleteAsync();
        return Results.NoContent();
    }

}
