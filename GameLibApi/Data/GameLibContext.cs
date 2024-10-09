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
    public DbSet<RGameGenre> RGameGenres => Set<RGameGenre>();
    public DbSet<RGamePlatform> RGamePlatforms => Set<RGamePlatform>();

     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //There can be games with same names but cannot be genres or platforms since they are categories dumb
        //But how to prevent posting games that already exists in the database
        //I will be the one who posts these games no body else will ever do that 
        //so it makes sense to check if the game name exists on the code rather than enforcing database to create indices for names -_- 
        //Also catalog names doesnt have to be unique
        modelBuilder.Entity<Genre>()
            .HasIndex(genre => genre.Name)
            .IsUnique();

        modelBuilder.Entity<Platform>()
            .HasIndex(platform => platform.Name)
            .IsUnique();
        
        // Many-to-Many: Game <-> Genre
        modelBuilder.Entity<RGameGenre>()
            .HasKey(rGameGenre => new { rGameGenre.GameId, rGameGenre.GenreId });

        modelBuilder.Entity<RGameGenre>()
            .HasOne(rGameGenre => rGameGenre.Game)
            .WithMany(game => game.RGameGenres)
            .HasForeignKey(rGameGenre => rGameGenre.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RGameGenre>()
            .HasOne(rGameGenre => rGameGenre.Genre)
            .WithMany(genre => genre.RGameGenres)
            .HasForeignKey(rGameGenre => rGameGenre.GenreId)
            .OnDelete(DeleteBehavior.Cascade);
        // Many-to-Many: Game <-> Platform
        modelBuilder.Entity<RGamePlatform>()
            .HasKey(rGamePlatform => new { rGamePlatform.GameId, rGamePlatform.PlatformId });

        modelBuilder.Entity<RGamePlatform>()
            .HasOne(rGamePlatform => rGamePlatform.Game)
            .WithMany(game => game.RGamePlatforms)
            .HasForeignKey(rGamePlatform => rGamePlatform.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RGamePlatform>()
            .HasOne(rGamePlatform => rGamePlatform.Platform)
            .WithMany(platform => platform.RGamePlatforms)
            .HasForeignKey(rGamePlatform => rGamePlatform.PlatformId)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-Many: Game <-> Catalog
        modelBuilder.Entity<RGameCatalog>()
            .HasKey(rGameCatalog => new { rGameCatalog.GameId, rGameCatalog.CatalogId });

        modelBuilder.Entity<RGameCatalog>()
            .HasOne(rGameCatalog => rGameCatalog.Game)
            .WithMany(game => game.RGameCatalogs)
            .HasForeignKey(rGameCatalog => rGameCatalog.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RGameCatalog>()
            .HasOne(rGameCatalog => rGameCatalog.Catalog)
            .WithMany(catalog => catalog.RGameCatalogs)
            .HasForeignKey(rGameCatalog => rGameCatalog.CatalogId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
