import 'package:flutter/material.dart';

import 'games.dart';

class Home extends StatefulWidget{
  const Home({super.key});
  @override
  State<Home> createState()=> _Home();  
}
class _Home extends State<Home>{
    var currentSubPageIndex = 0;
    @override
    Widget build(BuildContext context){
      var colorScheme = Theme.of(context).colorScheme; 

      Widget page;
      switch (currentSubPageIndex) {
        case 0:
          page = const Games();
        case 1:
          page = const Scaffold();
        default:
          throw UnimplementedError('no widget for $currentSubPageIndex');
      }
      var mainArea = ColoredBox(
        color: colorScheme.secondary,
        child: AnimatedSwitcher(
          duration: const Duration(milliseconds: 200),
          child: page,
        ),
      );
      return Scaffold(
        body: LayoutBuilder(
          builder: (context, constraints){
            return Row(
              children: [
                SafeArea(
                  child: NavigationRail(
                    extended: constraints.maxWidth >= 600,
                    destinations: const [
                      NavigationRailDestination(
                        icon: Icon(Icons.home),
                        label: Text('Home'),
                      ),
                      NavigationRailDestination(
                        icon: Icon(Icons.favorite),
                        label: Text('Favorites'),
                      ),
                    ],
                    selectedIndex: currentSubPageIndex,
                    onDestinationSelected: (value) {
                      setState(() {
                        currentSubPageIndex = value;
                      });
                    },
                  ),
                ),
                Expanded(child: mainArea),
              ],
            );
          }
          ),
      );
    }
}
