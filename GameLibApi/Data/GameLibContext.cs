using System;
using GameLibApi.Entities;
using Microsoft.EntityFrameworkCore;
namespace GameLibApi.Data;

public class GameLibContext(DbContextOptions<GameLibContext> options) :DbContext(options)
{   
    public DbSet<Game> Games => Set<Game>();

    public DbSet<Genre> Genres => Set<Genre>();

    public DbSet<Platform> Platforms => Set<Platform>();
     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       
    }
}
