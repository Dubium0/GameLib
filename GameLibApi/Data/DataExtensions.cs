using System;
using Microsoft.EntityFrameworkCore;

namespace GameLibApi.Data;

public static class DataExtensions
{
  public static async Task MigrateDbAsync(this WebApplication app){
        
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameLibContext>();
        await dbContext.Database.MigrateAsync();

    }
}
