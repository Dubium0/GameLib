class GameModel {
  
  final int id;
  final String name;
  final List<int> genreIds;
  final List<int> platformIds;
  final String releaseDate;
  final int metaCritic;
  final String backgroundImageUrl;

  GameModel({
    required this.id,
    required this.name,
    required this.genreIds,
    required this.platformIds,
    required this.releaseDate,
    required this.metaCritic,
    required this.backgroundImageUrl,
  });

  // Factory constructor to create a GameModel from a JSON map
  factory GameModel.fromJson(Map<String, dynamic> json) {
    return GameModel(
      id: json['id'],
      name: json['name'],
      genreIds: List<int>.from(json['genreId']),
      platformIds: List<int>.from(json['platformId']),
      releaseDate: json['releaseDate'],
      metaCritic: json['metaCritic'],
      backgroundImageUrl: json['backgroundImageUrl'],
    );
   }
}


class PagedGames{
  final int count;
  final bool hasPreviousPage;
  final bool hasNextPage;
  final List<GameModel> games;

  PagedGames(
    {
      required this.count,
      required this.hasPreviousPage,
      required this.hasNextPage,
      required this.games
    }
  );
  
  factory PagedGames.fromJson(Map<String, dynamic> json) {
    return PagedGames(
      count: json['count'],
      hasPreviousPage: json['hasPreviousPage'],
      hasNextPage: json['hasNextPage'],
      games: List<GameModel>.from(json['results'].map((json) => GameModel.fromJson(json as Map<String,dynamic>))),
    );
  }
}