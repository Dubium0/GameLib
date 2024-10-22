abstract class ModelFactory<T>{
  T fromJson(Map<String, dynamic> json);
}

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

class GameModelFactory extends ModelFactory<GameModel>{
  
  @override
  GameModel fromJson(Map<String, dynamic> json) {
    return GameModel.fromJson(json);
  }

}


abstract class ItemFilterInterface {
  String parseFilterParams();
}

class GameFilter implements ItemFilterInterface{
  @override
  String parseFilterParams() {
    return "";
  }

}

class PagedItem<T,TFactory extends ModelFactory<T>>{
  final int count;
  final bool hasPreviousPage;
  final bool hasNextPage;
  final List<T> items;

  PagedItem(
    {
      required this.count,
      required this.hasPreviousPage,
      required this.hasNextPage,
      required this.items
    }
  );
  
  factory PagedItem.fromJson(Map<String, dynamic> json,TFactory itemFactory) {
    return PagedItem(
      count: json['count'],
      hasPreviousPage: json['hasPreviousPage'],
      hasNextPage: json['hasNextPage'],
      items: List<T>.from(json['results'].map((json) => itemFactory.fromJson(json as Map<String,dynamic>))),
    );
  }
}

