
import 'dart:async';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import '../models/game_model.dart';
import '../services/fetch_data.dart';
import '../widgets/inf_box_grid_view.dart';
import 'package:cached_network_image/cached_network_image.dart';

class Games extends StatefulWidget{
  const Games({super.key});

  @override
  State<Games> createState() => _GamesState();

}

class _GamesState extends State<Games> {
  late final PagedDataFetcher<GameModel,GameModelFactory> gameDataFetcher;
  late Future<List<GameModel>> games_;
  late ScrollController controller;
  final int loadingDelaySeconds = 1;
  bool isLoading = false;
    

  @override
  void initState(){
    super.initState();
    
    gameDataFetcher = PagedDataFetcher(
                                        fetchUrl:'http://localhost:5291/api/games',
                                        filter: GameFilter(),
                                        itemFactory: GameModelFactory());
    
    gameDataFetcher.addBeforeFetchOperation( () {
      setState(() {
        isLoading = true;
      });
    });
     gameDataFetcher.addAfterFetchOperation( () {
      setState(() {
        isLoading = false;
      });
    });

    controller = ScrollController();



    controller.addListener(() {
      if (controller.hasClients) {
        var halfWay = (controller.position.minScrollExtent + controller.position.maxScrollExtent) /2;
        if (controller.position.pixels ==
            controller.position.maxScrollExtent && !isLoading) { 

            if(gameDataFetcher.isCacheFull){
              controller.jumpTo( halfWay );
            }
            setState(() {
              games_ = gameDataFetcher.fetchNextItems();
            });
        }else  if (controller.position.pixels ==
            controller.position.minScrollExtent && !isLoading) {
            if(gameDataFetcher.isCacheFull && gameDataFetcher.headPage.hasPreviousPage){
              controller.jumpTo( halfWay );
            }
            setState(() {
              games_ = gameDataFetcher.fetchPreviousItems();
            });
        }
      }
    });  
    games_ = gameDataFetcher.fetchNextItems(); 

  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor:const Color.fromARGB(255, 30, 30, 30),
      
      body: FutureBuilder(
      
        future : games_,
        builder: (context,  AsyncSnapshot<List<GameModel>> snapshot) {

          Widget child;
          if (snapshot.hasError) {
           child = Column(
            children :[
              const Icon(
                Icons.error_outline,
                color: Colors.red,
                size: 60,
              ),
              Padding(
                padding: const EdgeInsets.only(top: 16),
                child: Text('Error: ${snapshot.error}'),
              ),
            ]
          );
          }else if(snapshot.hasData){
            child = InfBoxGridView(
                    items  : snapshot.data!,
                    itemBuilder: GameItemFromFigma(),
                    scrollController: controller,
                    crossAxisCount:  clampDouble(MediaQuery.sizeOf(context).width / 400, 1,6).round(),
                    
                  );
            
          }else{
            child  =Column(
              children:[
                SizedBox(
                  width: 60,
                  height: 60,
                  child: CircularProgressIndicator(),
                ),
                Padding(
                  padding: EdgeInsets.only(top: 16),
                  child: Text('Awaiting result...'),
                ),
              ]
            );
          }
          return Center(
            child:child
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
      margin: const EdgeInsets.all(10),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(15.0),
        child: CachedNetworkImage(
                imageUrl: item.backgroundImageUrl,
                width: 250,  
                memCacheWidth: (250 * MediaQuery.of(context).devicePixelRatio).round(),
                imageBuilder: (context, imageProvider) => Container(
                  decoration: BoxDecoration(
                    image: DecorationImage(
                      image: imageProvider,
                      fit: BoxFit.cover,
                    ),
                  ),
                ),
                placeholder: (context, url) =>
                    const CircularProgressIndicator(),
                errorWidget: (context, url, error) => const Icon(Icons.error),
              ),
      )
    );
  }
}

class GameItemFromFigma implements IInfGridItemBuilder<GameModel>{
  @override
  Widget buildContent(BuildContext context, GameModel item) {
   return Center(
    
    child: Container(
      width: 250,
      height: 250,
      child: 
        Stack(
          children: [
            Positioned(
              left: 0,
              top: 0,
              child: CachedNetworkImage(
                      imageUrl: item.backgroundImageUrl,
                      width: 250, 
                      height: 175,
                      memCacheWidth: (250 * MediaQuery.of(context).devicePixelRatio).round(),
                      memCacheHeight: (175 * MediaQuery.of(context).devicePixelRatio).round(),
                      imageBuilder: (context, imageProvider) => Container(
                        decoration: ShapeDecoration(
                          image: DecorationImage(
                            image:imageProvider,
                            fit: BoxFit.fill,
                          ),
                          shape: const RoundedRectangleBorder(
                            borderRadius: BorderRadius.only(
                              topLeft: Radius.circular(20),
                              topRight: Radius.circular(20),
                            ),
                          ),
                        ),
                      ),
                      placeholder: (context, url) =>
                          const CircularProgressIndicator(),
                      errorWidget: (context, url, error) => const Icon(Icons.error),
                    ),
            ),
            Positioned(
              left: 208,
              top: 18,
              child: Container(
                width: 38,
                height: 38,
                decoration:  BoxDecoration(
                  image: DecorationImage(
                    image:   AssetImage('assets/icons/addToCollection.png'),
                    fit: BoxFit.fill,
                  ),
                ),
              ),
            ),
            Positioned(
              left: 0,
              top: 175,
              child: Container(
                width: 250,
                height: 75,
                child: Stack(
                  children: [
                    Positioned(
                      left: 0,
                      top: 0,
                      child: Container(
                        width: 250,
                        height: 75,
                        decoration: ShapeDecoration(
                          color: Color(0xFF2C2C2C),
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.only(
                              bottomLeft: Radius.circular(20),
                              bottomRight: Radius.circular(20),
                            ),
                          ),
                        ),
                      ),
                    ),
                    Positioned(
                      left: 23.75,
                      top: 27.50,
                      child: SizedBox(
                        width: 250,
                        height: 30,
                        child: Text(
                          item.name,
                          style: TextStyle(
                            color: Color(0xFFF5F5F5),
                            fontSize: 12,
                            fontFamily: 'Copperplate Gothic Bold',
                            fontWeight: FontWeight.w400,
                            height: 0,
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),

    ),



);


  }

}


