//flutter packages
import 'package:flutter/material.dart';
// created
import 'screens/home.dart';



void main() {
  runApp(const GameLibApp());
}

class GameLibApp extends StatelessWidget {
  const GameLibApp({super.key});

  @override
  Widget build(BuildContext context) {
    return const MaterialApp(
      home: Home(),
    );
  }
}



