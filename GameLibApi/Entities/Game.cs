

namespace GameLibApi.Entities;

    public class Game{
        public int Id { get; set;}
        public required string Name { get; set;}
        public DateOnly ReleaseDate { get; set;}
        public int MetaCritic {get; set;}
        public string? BackgroundImageUrl {get;set;}


        // Relationships
        public ICollection<RGameGenre>? RGameGenres { get; set; }
        public ICollection<RGamePlatform>? RGamePlatforms { get; set; }
        public ICollection<RGameCatalog>? RGameCatalogs { get; set; }
    }



