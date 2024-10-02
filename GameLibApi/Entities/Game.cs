

namespace GameLibApi.Entities;

    public class Game{
        public int Id { get; set;}
        public required string Name { get; set;}
        public int GenreId;
        public Genre? Genre;
        public int PlatformId;
        public Platform? Platform;
        public DateOnly ReleaseDate { get; set;}
        public int MetaCritic {get; set;}
        public string? BackgroundImageUrl {get;set;}

    }



