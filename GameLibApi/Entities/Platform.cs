

namespace GameLibApi.Entities;

    public class Platform{
        
        public int Id { get; set;}
        public required string Name { get; set;}




        // Relationships
        public ICollection<RGamePlatform>? RGamePlatforms { get; set; }
    }



