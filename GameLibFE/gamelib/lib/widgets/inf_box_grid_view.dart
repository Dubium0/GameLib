
import 'package:flutter/material.dart';

class InfBoxGridView<T> extends StatelessWidget {
  final int crossAxisCount;
  final double crossAxisSpacing;
  final double mainAxisSpacing;
  final ScrollController? scrollController;
  final List<T> items;
  final IInfGridItemBuilder<T> itemBuilder;
  const InfBoxGridView({super.key,
     required this.items, required this.itemBuilder,
      this.crossAxisCount = 5,this.crossAxisSpacing = 10.0,
       this.mainAxisSpacing = 10.0,this.scrollController});

  @override
  Widget build(BuildContext context) {
    return GridView.builder(
      controller: scrollController,
      gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: crossAxisCount,
        crossAxisSpacing: crossAxisSpacing,
        mainAxisSpacing: mainAxisSpacing,
      ),
      itemCount: items.length,
      itemBuilder: buildItem,
      shrinkWrap: true,
      
    );
  }

  Widget? buildItem(context, index){
      return itemBuilder.buildContent(context,items[index]);
  }
 
} 

abstract class IInfGridItemBuilder<T>{

  Widget buildContent(BuildContext context,T item);

}


