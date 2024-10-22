import 'dart:convert';
import 'package:http/http.dart' as http;
import '../models/game_model.dart';
import '../utils/item_cache.dart';



class PagedDataFetcher<T,TFactory extends ModelFactory<T>> {
  final ItemCache<T> _itemChache = ItemCache();
  final TFactory itemFactory;
  final int pageCount  = 200;
  final String fetchUrl;
  final ItemFilterInterface filter;
  
  int currentPageNumber = -1;// Initially no pages
  bool hasNextPage = true; 
  bool hasPreviousPage = false;

  PagedDataFetcher({required this.fetchUrl,required this.filter,required this.itemFactory});

  Future<PagedItem<T,TFactory>> fetchPage(int pageNumber) async { 
    int maxInt = 0x7FFFFFFFFFFFFFFF;
    pageNumber = pageNumber.clamp(0, maxInt);

    String filterStr = filter.parseFilterParams();
    final response = await http
      .get(Uri.parse('$fetchUrl?pageCount=$pageCount&pageNumber=$pageNumber&$filterStr'));

    if (response.statusCode == 200) {
      
      final Map<String,dynamic> responseBody = jsonDecode(response.body) as Map<String,dynamic>;
      currentPageNumber = pageNumber;
      return PagedItem.fromJson(responseBody,itemFactory);

    } else {
      throw Exception('Failed to load page');
    }

  }
  Future<List<T>> fetchNextItems() async {
     if(hasNextPage){ 
      var pagedItems = await fetchPage(currentPageNumber + 1);
      hasNextPage = pagedItems.hasNextPage;
      hasPreviousPage = pagedItems.hasPreviousPage;
      _itemChache.addItemsCached(pagedItems.items, Direction.tail);
    }
    return _itemChache.items;   
  }
  Future<List<T>> fetchPreviousItems() async {
     if(hasNextPage){
      var pagedItems = await fetchPage(currentPageNumber -1);
      hasNextPage = pagedItems.hasNextPage;
      hasPreviousPage = pagedItems.hasPreviousPage;
      _itemChache.addItemsCached(pagedItems.items, Direction.head);
    }
    return _itemChache.items;   
  }


}

