using System;
using GameLibApi.Data;
using GameLibApi.Dtos.PlatformDtos;
using GameLibApi.Entities;
using GameLibApi.Mappings;
using Microsoft.EntityFrameworkCore;

namespace GameLibApi.EndPoints;

public static class PlatformsEndPoints
{
    private const string GetplatformEndpointName = "GetPlatform";
    public static RouteGroupBuilder MapPlatformsEndpoints(this WebApplication app){

        var group = app.MapGroup("api/platforms").WithParameterValidation();

        group.MapGet("/", async (GameLibContext dbContext) => {
            return await dbContext.Platforms
                .Select(platform => platform.ToDto())
                .AsNoTracking()
                .ToListAsync();
        });

        group.MapGet("/{id}", async (int id, GameLibContext dbContext)=>{

            var existingPlatform = await dbContext.Platforms.FindAsync(id);

            if(existingPlatform == null){
                return Results.NotFound();
            }
            return Results.Ok(existingPlatform.ToDto());
        }).WithName(GetplatformEndpointName);

        group.MapPut("/{id}", async (int id, UpdatePlatformDto updatedPlatformDto,GameLibContext dbContext)=>{

            var existingPlatform = await dbContext.Platforms.FindAsync(id);

            if (existingPlatform == null){
                return Results.NotFound();
            }

            dbContext.Entry(existingPlatform)
                .CurrentValues
                .SetValues(updatedPlatformDto.ToEntity(id));

            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapPost("/", async (CreatePlatformDto newPlatformDto,GameLibContext dbContext)=>{

            Platform platform =  newPlatformDto.ToEntity();
           
            dbContext
                .Platforms
                .Add(platform);
                
            await dbContext
                .SaveChangesAsync();

            return Results.CreatedAtRoute(GetplatformEndpointName, 
            new { id = platform.Id}, platform.ToDto());

        });
        
        group.MapDelete("/{id}", async (int id, GameLibContext dbContext) =>{ 
            // batch delete
            await dbContext.Platforms
                .Where(platform => platform.Id == id)
                .ExecuteDeleteAsync();
            return Results.NoContent();
        });


        return group;

    }
}
