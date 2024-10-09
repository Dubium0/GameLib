using System;
using System.Diagnostics;
using System.Transactions;
using GameLibApi.Data;
using GameLibApi.Dtos.GenreDtos;
using GameLibApi.Entities;
using GameLibApi.Mappings;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GameLibApi.EndPoints;

public static class GenresEndPoints
{
    private const string GetGenreEndpointName = "GetGenre";
    public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app){

        var group = app.MapGroup("api/genres").WithParameterValidation();

        group.MapGet("/", async (GameLibContext dbContext) => {
            return await dbContext.Genres
                .Select(genre => genre.ToDto())
                .AsNoTracking()
                .ToListAsync();
        });

        group.MapGet("/{id}", async (int id, GameLibContext dbContext)=>{

            var existingGenre = await dbContext.Genres.FindAsync(id);

            if(existingGenre == null){
                return Results.NotFound();
            }
            return Results.Ok(existingGenre.ToDto());
        }).WithName(GetGenreEndpointName);

        group.MapPut("/{id}", async (int id, UpdateGenreDto updatedGenreDto,GameLibContext dbContext)=>{

            var existingGenre = await dbContext.Genres.FindAsync(id);

            if (existingGenre == null){
                return Results.NotFound();
            }

            using (var transaction = await dbContext.Database.BeginTransactionAsync()){
                try{

                    dbContext.Entry(existingGenre)
                        .CurrentValues
                        .SetValues(updatedGenreDto.ToEntity(id));

                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Results.NoContent();
                }catch(Exception e){
                    await transaction.RollbackAsync();
                
                    Debug.WriteLine($"Exception: {e} occured and transaction rolled back ");

                    return TransactionExceptionHandler.Handle(e);

                }
            }
        });

        group.MapPost("/", async (CreateGenreDto newGenreDto,GameLibContext dbContext)=>{

            Genre genre =  newGenreDto.ToEntity();
           
            using (var transaction = await dbContext.Database.BeginTransactionAsync() ){
                try{

                dbContext
                    .Genres
                    .Add(genre);
                    
                await dbContext
                    .SaveChangesAsync();

                await transaction.CommitAsync();
                }
                catch(Exception e){
                    await transaction.RollbackAsync();
                
                    Debug.WriteLine($"Exception: {e} occured and transaction rolled back ");

                    return TransactionExceptionHandler.Handle(e);
                }
            }

            return Results.CreatedAtRoute(GetGenreEndpointName, 
            new { id = genre.Id}, genre.ToDto());

        });
        
        group.MapDelete("/{id}", async (int id, GameLibContext dbContext) =>{ 
            // batch delete
            await dbContext.Genres
                .Where(genre => genre.Id == id)
                .ExecuteDeleteAsync();
            return Results.NoContent();
        });


        return group;

    }
}
