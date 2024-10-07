

namespace GameLibApi.Entities;

    public class Genre{
        
        public int Id { get; set;}
        public required string Name { get; set;}
        

        // Relationships
         public ICollection<RGameGenre>? RGameGenres { get; set; }

    }



