import 'package:flutter/material.dart';

import '../utils/random_color.dart';
import '../widgets/inf_box_grid_view.dart';

class Games extends StatefulWidget{
  const Games({super.key});

  @override
  State<Games> createState() => _Gamestate();

}

class _Gamestate extends State<Games> {
  final List<String> colorList = <String>[];
  String randomColorName = "";
  Future<List<String>> futureColor() async {
    await Future.delayed(
      const Duration(seconds: 1),
      () {
        randomColorName = RandomColorExtension.randomColorHex();
        colorList.add(randomColorName);
        debugPrint("HexColor: $randomColorName");
        setState(() {});
      },
    );
    return colorList;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: FutureBuilder(
        future: futureColor(),
        builder: (context, snapshot) {
          if (snapshot.data == null) {
            return const Center(
              child: CircularProgressIndicator(),
            );
          }
          return InfBoxGridView(
            items  : colorList,
            itemBuilder: RandomColorItem(),
          );
        },
      ),
    );
  }
}


class RandomColorItem  implements IInfGridItemBuilder<String>{
  
  @override
  Widget buildContent(BuildContext context, item) {
     return Card(
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(15.0),
        ),
        elevation: 8,
        color: Color(int.parse(item)),
        child: Center(
          child: SelectableText(
            item,
            cursorColor: Colors.blue,
            contextMenuBuilder: (context, editableTextState) {
              return AdaptiveTextSelectionToolbar.buttonItems(
                anchors: editableTextState.contextMenuAnchors,
                buttonItems: <ContextMenuButtonItem>[
                  ContextMenuButtonItem(
                    onPressed: () {
                      editableTextState
                          .copySelection(SelectionChangedCause.toolbar);
                    },
                    type: ContextMenuButtonType.copy,
                  ),
                ],
              );
            },
            showCursor: true,
            style: const TextStyle(
              fontSize: 24,
              fontWeight: FontWeight.bold,
              color: Colors.white,
            ),
          ),
        ),
      );
  }
}

