namespace GameLibApi.Entities;

    public class RGameCatalog{
        
        public int GameId { get; set;}
        public Game? Game { get; set; }
        
        public int CatalogId { get; set;}
        public Catalog? Catalog {get; set;}

    }


