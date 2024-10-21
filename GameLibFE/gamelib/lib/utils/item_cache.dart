
enum Direction {head, tail}
class ItemCache <T> {
  List<T> _items = List.empty(growable: true);
  late final int _maxItemCount;
  ItemCache({int maxItemCount = 200}){
    _maxItemCount = maxItemCount;
  }

  void addItemsCached(List<T> items,Direction addDirection){
    var difference =(items.length + _items.length) - _maxItemCount ;
    var shouldRemoveItemsFromList = difference > 0;
    if(shouldRemoveItemsFromList ){  
      _removeItemsFrom(difference, addDirection == Direction.head ? Direction.tail : Direction.head);
    }
    _addItemsFrom(items, addDirection);
  }

  void _removeItemsFrom(int removeCount, Direction removeDirection){
    switch(removeDirection){
      case Direction.head:
        _items.removeRange(0, removeCount);
      break;
      case Direction.tail:
        _items.removeRange(_items.length- removeCount, _items.length);
      break;    
    }
  }
  void _addItemsFrom(List<T> itemsToAdd, Direction addDirection){
    switch(addDirection){
      case Direction.head:
        _items = List.from(itemsToAdd)..addAll(_items);
      break;
      case Direction.tail:
        _items.addAll(itemsToAdd);
      break;    
    }
  }
  List<T> get items => _items;
  int get maxItemCount => _maxItemCount;

}