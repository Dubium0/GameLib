import 'dart:math';

extension RandomColorExtension on String {
  static const letters = "0123456789abcdefghijklmnopqrstuvwxyz";

  static String _randomString() {
    final Random random = Random();
    String result = '';

    for (int i = 0; i < 6; i++) {
      int randomIndex = random.nextInt(letters.length);
      result += letters[randomIndex];
    }
    return result;
  }

  static String randomColorHex() {
    String hexColor;
    do {
      hexColor = _randomString();
    } while (!RegExp(r'^[0-9a-fA-F]{6}$').hasMatch(hexColor));

    return "0xFF$hexColor";
  }
}