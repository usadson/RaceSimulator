using Model;
using System;

namespace Controller
{
    public class CharacterData
    {
        private static Dictionary<Character, CharacterData> _dict = new()
        {
            {Character.BabyDaisy, new CharacterData{Color = ConsoleColor.DarkYellow}},
            {Character.BabyLuigi, new CharacterData{Color = ConsoleColor.Green}},
            {Character.BabyMario, new CharacterData{Color = ConsoleColor.Red}},
            {Character.BabyPeach, new CharacterData{Color = ConsoleColor.Magenta}},
            {Character.Birdo, new CharacterData{Color = ConsoleColor.Cyan}},
            {Character.Daisy, new CharacterData{Color = ConsoleColor.DarkYellow}},
            {Character.Daisy, new CharacterData{Color = ConsoleColor.Yellow}},
        };

        public static CharacterData Of(Character character) => _dict[character];

        public ConsoleColor Color { get; init; }

        private CharacterData()
        {
        }
    }
}
