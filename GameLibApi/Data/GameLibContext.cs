using System;
using GameLibApi.Entities;
using Microsoft.EntityFrameworkCore;
namespace GameLibApi.Data;

public class GameLibContext(DbContextOptions<GameLibContext> options) :DbContext(options)
{   
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<Catalog> Catalogs => Set<Catalog>();
    public DbSet<RGameCatalog> RGameCatalogs => Set<RGameCatalog>();
    public DbSet<RGameGenre> RGameGenre => Set<RGameGenre>();
    public DbSet<RGamePlatform> RGamePlatforms => Set<RGamePlatform>();

     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
         // Many-to-Many: Game <-> Genre
    modelBuilder.Entity<RGameGenre>()
        .HasKey(gg => new { gg.GameId, gg.GenreId });

    modelBuilder.Entity<RGameGenre>()
        .HasOne(gg => gg.Game)
        .WithMany(g => g.RGameGenres)
        .HasForeignKey(gg => gg.GameId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<RGameGenre>()
        .HasOne(gg => gg.Genre)
        .WithMany(g => g.RGameGenres)
        .HasForeignKey(gg => gg.GenreId)
        .OnDelete(DeleteBehavior.Cascade);
    // Many-to-Many: Game <-> Platform
    modelBuilder.Entity<RGamePlatform>()
        .HasKey(gp => new { gp.GameId, gp.PlatformId });

    modelBuilder.Entity<RGamePlatform>()
        .HasOne(gp => gp.Game)
        .WithMany(g => g.RGamePlatforms)
        .HasForeignKey(gp => gp.GameId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<RGamePlatform>()
        .HasOne(gp => gp.Platform)
        .WithMany(p => p.RGamePlatforms)
        .HasForeignKey(gp => gp.PlatformId)
        .OnDelete(DeleteBehavior.Cascade);

    // Many-to-Many: Game <-> Catalog
    modelBuilder.Entity<RGameCatalog>()
        .HasKey(gc => new { gc.GameId, gc.CatalogId });

    modelBuilder.Entity<RGameCatalog>()
        .HasOne(gc => gc.Game)
        .WithMany(g => g.RGameCatalogs)
        .HasForeignKey(gc => gc.GameId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<RGameCatalog>()
        .HasOne(gc => gc.Catalog)
        .WithMany(c => c.RGameCatalogs)
        .HasForeignKey(gc => gc.CatalogId)
        .OnDelete(DeleteBehavior.Cascade);

    }
}
