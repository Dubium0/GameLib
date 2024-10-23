
import 'dart:convert';

import 'package:http/http.dart' as http;
import '../models/game_model.dart';
import '../utils/item_cache.dart';


class PageLiteInfo{
  final bool hasNextPage;
  final bool hasPreviousPage;
  final int pageNumber;
  const PageLiteInfo({required this.hasNextPage, required this.hasPreviousPage, required this.pageNumber});
}

class PagedDataFetcher<T,TFactory extends ModelFactory<T>> {
  final TFactory itemFactory;
  final int pageCount  = 100;
  late final ItemCache<T> _itemChache;
  final String fetchUrl;
  final ItemFilterInterface filter;
  final List<void Function()> beforeFetchOperations = List.empty(growable: true);
  final List<void Function()> afterFetchOperations= List.empty(growable: true);

  PageLiteInfo tailPage = const PageLiteInfo(hasNextPage: true, hasPreviousPage: false, pageNumber: -1);
  PageLiteInfo headPage = const PageLiteInfo(hasNextPage: true, hasPreviousPage: false, pageNumber: -1);
  int fetchCount = 0;
  PagedDataFetcher({required this.fetchUrl,required this.filter,required this.itemFactory}){
    _itemChache = ItemCache(maxItemCount: pageCount*2);
  }

  bool get isCacheFull => _itemChache.isFull;

  void addBeforeFetchOperation(void Function() operation){
    beforeFetchOperations.add(operation);
  }
  void addAfterFetchOperation(void Function() operation){
    afterFetchOperations.add(operation);
  }

  void _executeBeforeFetchOperations(){
    for (var operation in beforeFetchOperations){
      operation();
    }
  }
  void _executeAfterFetchOperations(){
    for (var operation in afterFetchOperations){
      operation();
    }
  }


  Future<PagedItem<T,TFactory>> fetchPage(int pageNumber) async { 
    int maxInt = 0x7FFFFFFFFFFFFFFF;
    pageNumber = pageNumber.clamp(0, maxInt);

    String filterStr = filter.parseFilterParams();
    _executeBeforeFetchOperations();
    final response = await http
      .get(Uri.parse('$fetchUrl?pageCount=$pageCount&pageNumber=$pageNumber&$filterStr'));
    fetchCount ++;
    _executeAfterFetchOperations();
    if (response.statusCode == 200) {
      
      final Map<String,dynamic> responseBody = jsonDecode(response.body) as Map<String,dynamic>;
    
      return PagedItem.fromJson(responseBody,itemFactory);

    } else {
      throw Exception('Failed to load page');
    }
  }
  Future<List<T>> fetchNextItems() async {
   
    if(tailPage.hasNextPage){ 
      var pagedItems = await fetchPage(tailPage.pageNumber + 1);
      headPage = tailPage;
      tailPage = PageLiteInfo(hasNextPage:  pagedItems.hasNextPage, hasPreviousPage: pagedItems.hasPreviousPage, pageNumber: tailPage.pageNumber + 1);
      if(fetchCount == 1){
        headPage = tailPage;
      }
      _itemChache.addItemsCached(pagedItems.items, Direction.tail);
    }
   
    return _itemChache.items;   
  }
  Future<List<T>> fetchPreviousItems() async {
    
     if(headPage.hasPreviousPage){
      var pagedItems = await fetchPage(headPage.pageNumber -1);
      tailPage = headPage;
      headPage = PageLiteInfo(hasNextPage:  pagedItems.hasNextPage, hasPreviousPage: pagedItems.hasPreviousPage, pageNumber: tailPage.pageNumber + 1);

      _itemChache.addItemsCached(pagedItems.items, Direction.head);
    }

    return _itemChache.items;   
  }


}

