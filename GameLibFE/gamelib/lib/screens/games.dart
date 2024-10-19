
import 'package:flutter/material.dart';
import '../models/game_model.dart';
import 'dart:convert';
import 'package:http/http.dart' as http;
import '../widgets/inf_box_grid_view.dart';

class Games extends StatefulWidget{
  const Games({super.key});

  @override
  State<Games> createState() => _Gamestate();

}

Future<PagedGames> fetchPagedGames(int pageNumber, int pageCount) async {
  final response = await http
      .get(Uri.parse('http://localhost:5291/api/games?pageCount=$pageCount&pageNumber=$pageNumber'));

  if (response.statusCode == 200) {
    // If the server did return a 200 OK response,
    // then parse the JSON.
     // Parse the response body as JSON
    final Map<String,dynamic> responseBody = jsonDecode(response.body) as Map<String,dynamic>;

      // Convert the JSON into a list of GameModel instances
    return PagedGames.fromJson(responseBody);

  } else {
    // If the server did not return a 200 OK response,
    // then throw an exception.
    throw Exception('Failed to load album');
  }
}


class _Gamestate extends State<Games> {
  List<GameModel> games = List.empty(growable: true);
  late Future<List<GameModel>> games_;
  final pageCount = 20;
  var pageNumber = 0;
  bool hasNextPage = true;
  late ScrollController controller;


  Future<List<GameModel>> fetchGames() async{
    if(hasNextPage){
      var pagedGames = await fetchPagedGames(pageNumber, pageCount);
      hasNextPage = pagedGames.hasNextPage;
      pageNumber ++;
      games.addAll(pagedGames.games);
    }
    return games;   
  }


  @override
  void initState(){
    super.initState();
    controller = ScrollController();

    controller.addListener(() {
      if (controller.hasClients) {
        if (controller.position.maxScrollExtent == controller.offset) {
          setState(() {
            games_ = fetchGames();
          });
        }
      }
    });  
    games_ = fetchGames(); 

  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: FutureBuilder(
        future : games_,
        builder: (context,  AsyncSnapshot<List<GameModel>> snapshot) {

          List<Widget> children;
          if (snapshot.hasError) {
           children = <Widget>[
              const Icon(
                Icons.error_outline,
                color: Colors.red,
                size: 60,
              ),
              Padding(
                padding: const EdgeInsets.only(top: 16),
                child: Text('Error: ${snapshot.error}'),
              ),
            ];
          }else if(snapshot.hasData){
            return
              InfBoxGridView(
                  items  : snapshot.data!,
                  itemBuilder: GameItem(),
                  scrollController: controller,
                );
          }else{
            children  = const <Widget>[
              SizedBox(
                width: 60,
                height: 60,
                child: CircularProgressIndicator(),
              ),
              Padding(
                padding: EdgeInsets.only(top: 16),
                child: Text('Awaiting result...'),
              ),
            ];
          }
          return Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: children,
            ),
          );
        },
      ),
    );
  }
}

class GameItem implements IInfGridItemBuilder<GameModel>{
  @override
  Widget buildContent(BuildContext context, GameModel item) {
    
    return Card(
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(15.0),
      ),
      elevation: 8,
      child: Center(
        child: Image.network(
          item.backgroundImageUrl,
          fit: BoxFit.cover,
          height: 100, // Adjust the size as needed
          width: 100,
          loadingBuilder: (context, child, loadingProgress) {
            if (loadingProgress == null) return child;
            return Center(
              child: CircularProgressIndicator(
                value: loadingProgress.expectedTotalBytes != null
                    ? loadingProgress.cumulativeBytesLoaded / (loadingProgress.expectedTotalBytes ?? 1)
                    : null,
              ),
            );
          },
          errorBuilder: (context, error, stackTrace) => const Icon(Icons.error), // In case of an error loading the image
        ),
      )
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

