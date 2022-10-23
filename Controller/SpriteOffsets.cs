using System.Drawing;
using Model;

namespace Controller
{
    public static class SpriteOffsets
    {
        public static readonly Dictionary<Character, Rectangle> MinimapOffsets = new()
        {
            { Character.BabyDaisy, new(7, 2, 47, 64) },
            { Character.BabyLuigi, new(62, 2, 52, 64) },
            { Character.BabyMario, new(118, 2, 56, 64) },
            { Character.BabyPeach, new(184, 2, 46, 64) },
            { Character.Birdo, new(297, 2, 63, 64) },
            { Character.Daisy, new(366, 2, 62, 64) },
            { Character.DiddyKong, new(434, 2, 63, 64) },
            { Character.DonkeyKong, new(504, 2, 61, 64) },
            { Character.FunkyKong, new(572, 2, 64, 64) },


            { Character.DryBowser, new(122, 72, 64, 64) },
            { Character.DryBones, new(194, 76, 48, 60) },
            { Character.Toadette, new(247, 73, 55, 63) },
            { Character.Toad, new(308, 73, 55, 63) },
            { Character.Bowser, new(363, 72, 64, 64) },
            { Character.BowserJunior, new(434, 72, 61, 64) },
            { Character.Luigi, new(511, 72, 48, 64) },
            { Character.Mario, new(576, 73, 55, 63) },

            { Character.KoopaTroopa, new(202, 146, 54, 59) },
            { Character.Peach, new(328, 142, 59, 63) },
            { Character.Rosalina, new(391, 142, 51, 64) },
            { Character.KingBoo, new(446, 145, 63, 61) },
            { Character.Waluigi, new(514, 143, 55, 62) },
            { Character.Wario, new(572, 142, 64, 63) },

            { Character.Yoshi, new(6, 212, 54, 60) },
        };
    }
}
