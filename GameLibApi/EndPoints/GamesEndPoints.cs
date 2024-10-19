using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks.Dataflow;
using GameLibApi.Data;
using GameLibApi.Dtos;
using GameLibApi.Dtos.GameDtos;
using GameLibApi.Entities;
using GameLibApi.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
namespace GameLibApi.EndPoints;

public static class GamesEndPoints
{
    public const string GetGameEndpointName = "GetGame";
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app){
        var group = app.MapGroup("api/games").WithParameterValidation();

        group.MapGet("/", GetGamesWithDetails);

        group.MapGet("/{id}", GetGameById ).WithName(GetGameEndpointName);

        group.MapPost("/", PostGame );

        group.MapPut("/{id}",UpdateGame );

        group.MapDelete("/{id}", DeleteGame );

        return group;
    }

    public static async Task<IResult> GetGamesWithDetails(GameLibContext dbContext,[AsParameters] GetGamesDto filters){

        IQueryable<Game> games = dbContext.Games.AsNoTracking();
        IQueryable<RGameGenre> rGameGenres = dbContext.RGameGenres.AsNoTracking();
        IQueryable<RGamePlatform> rGamePlatforms = dbContext.RGamePlatforms.AsNoTracking();

        if(!string.IsNullOrEmpty(filters.Metacritic)){
                
            var filter =  MetacriticFilterGenerator(filters.Metacritic);
            if( filter == null){
                return Results.BadRequest($"Given metacritic filter is in wrong format! {filters.Metacritic}");
            }
            games = games.Where( filter );
            }

        if(!string.IsNullOrEmpty(filters.Dates)){
            var filter = DateFilterGenerator(filters.Dates);
            if( filter == null){
                return Results.BadRequest($"Given date filter is in wrong format! {filters.Dates}");
            }
            games = games.Where( filter );
        }
            
        if(!string.IsNullOrEmpty(filters.Genre)){
            var filter = GameGenreFilterGenerator(filters.Genre);
            if(filter == null){
                return Results.BadRequest($"Given genre filter is in wrong format! {filters.Genre}");
            }

            games = from game in games
                    join rGameGenre in rGameGenres.Where(filter) on game.Id equals rGameGenre.GameId
                    select game;
        }
        

        if(!string.IsNullOrEmpty(filters.Platform)){
            var filter = GamePlatformFilterGenerator(filters.Platform);
            if(filter == null){
                 return Results.BadRequest($"Given genre filter is in wrong format! {filters.Genre}");
            }
            games = from game in games
                    join rGamePlatform in rGamePlatforms.Where( filter ) on game.Id equals rGamePlatform.GameId
                    select game;
        }

        if(!string.IsNullOrEmpty(filters.Orderings)){
            // will try to find generalize this code
            bool failed = false;
            switch(filters.Orderings){
                case "date":
                    games = games.OrderBy(game => game.ReleaseDate);
                break;
                case "-date":
                    games = games.OrderByDescending(game => game.ReleaseDate);
                break;
                case "metacritic":
                    games = games.OrderBy(game => game.MetaCritic);
                break;
                case "-metacritic":
                    games = games.OrderByDescending(game => game.MetaCritic);
                break;
                default:
                    failed = true;
                break;
           }
           if(failed){
                return Results.BadRequest($"Given ordering filter is in wrong format! {filters.Orderings}");
           }
        }

        int pageNumber = 0;
        int pageCount = 200;
        if( filters.PageNumber != null){
            pageNumber =(int)filters.PageNumber;
        }
        
        if( filters.PageCount != null){
            pageCount = (int)filters.PageCount;
        }    

        var resultQuery = games.AsNoTracking()
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
        );
        
                        
        bool hasPreviousPage = pageNumber > 0;
        bool hasNextPage = pageCount * pageNumber < resultQuery.Count(); 

        return Results.Ok( new PageDto<GameDetailsDto>(
                             resultQuery.Count(),
                             hasPreviousPage,
                             hasNextPage,
                             await resultQuery
                            .Skip(pageNumber * pageCount)
                            .Take(pageCount)
                            .ToListAsync()
        )
           
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

    
    
    public static Expression<Func<Game,bool>>? MetacriticFilterGenerator( string metacritic ){

            var metacriticSplitted = metacritic.Split(',');

            switch (metacriticSplitted.Count()){
                case 1:
                    {
                        int metacriticRating;
                        if( Int32.TryParse( metacriticSplitted[0], out metacriticRating)){
                            
                            return  game => game.MetaCritic  == metacriticRating;
                        }
                    }
                break;
                case 2:
                    {
                        int left;
                        int right;
                        if( Int32.TryParse( metacriticSplitted[0], out left) && Int32.TryParse( metacriticSplitted[1], out right)){
                           if(left > right){
                                var temp = right;
                                right = left;
                                left = temp;
                           }
                            
                            return  game => game.MetaCritic  >  left  && game.MetaCritic  <  right ;;
                        }
                       
                    }
                break;

                default:
                break;
            }
            return null;

            
    }

    public static Expression<Func<Game,bool>>? DateFilterGenerator( string date ){
        var dateSplitted = date.Split(',');

        switch(dateSplitted.Count()){
            case 2:
                DateOnly left;
                DateOnly right;
                if( DateOnly.TryParse(dateSplitted[0], out left) && DateOnly.TryParse(dateSplitted[1], out right)){
                    if(left > right){
                        var temp = right;
                        right= left;
                        left = temp;
                    }
                    return game => game.ReleaseDate > left && game.ReleaseDate < right;
                    
                }
            break;
            default:
            break;
        } 

        return null;

    }

    public static Expression<Func<RGameGenre,bool>>? GameGenreFilterGenerator(string genre){
        var genreSplitted = genre.Split(',');
        
        List<int> genreIds = new List<int>();

        foreach( var genreIdStr in genreSplitted ){
            if(Int32.TryParse(genreIdStr, out var index)){
                genreIds.Add(index);
            }else{
                return null;
            }
        }

        return rGameGenre => genreIds.Contains(rGameGenre.GenreId);
    }

    public static Expression<Func<RGamePlatform,bool>>? GamePlatformFilterGenerator(string platform){
        var platformSplitted = platform.Split(',');
        
        List<int> platformIds = new List<int>();

        foreach( var platformIdStr in platformSplitted ){
            if(Int32.TryParse(platformIdStr, out var index)){
                platformIds.Add(index);
            }else{
                return null;
            }
        }

        return rGamePlatform => platformIds.Contains(rGamePlatform.PlatformId);
    }

}   

