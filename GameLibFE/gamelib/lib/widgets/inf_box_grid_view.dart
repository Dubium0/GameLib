
import 'package:flutter/material.dart';

class InfBoxGridView<T> extends StatelessWidget {
  final int crossAxisCount;
  final double crossAxisSpacing;
  final double mainAxisSpacing;
  final List<T> items;
  final IInfGridItemBuilder<T> itemBuilder;
  const InfBoxGridView({super.key, required this.items, required this.itemBuilder, this.crossAxisCount = 5,this.crossAxisSpacing = 10.0, this.mainAxisSpacing = 10.0});

  @override
  Widget build(BuildContext context) {
    return GridView.builder(
      gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: crossAxisCount,
        crossAxisSpacing: crossAxisSpacing,
        mainAxisSpacing: mainAxisSpacing,
      ),
      itemCount: items.length,
      itemBuilder: buildItem,
    );
  }

  Widget? buildItem(context, index){
      return itemBuilder.buildContent(context,items[index]);
  }
 
} 

abstract class IInfGridItemBuilder<T>{

  Widget buildContent(BuildContext context,T item);

}


