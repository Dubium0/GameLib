

namespace GameLibApi.Entities;

    public class Catalog{
        
        public int Id { get; set;}
        public required string Name { get; set;}
        public bool visibility {get; set;}

        // Relationships
        public ICollection<RGameCatalog>? RGameCatalogs { get; set; }

    }



