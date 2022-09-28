using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model;
using SFML.Graphics;

namespace Controller
{
    public static class SpriteOffsets
    {
        public static readonly Dictionary<Character, IntRect> MinimapOffsets = new()
        {
            { Character.BabyDaisy, new(14, 6, 25, 30) },
            { Character.BabyLuigi, new(41, 7, 28, 29) },
            { Character.BabyMario, new(71, 6, 30, 30) },
            { Character.BabyPeach, new(104, 5, 25, 31) },
            { Character.Birdo, new(131, 7, 24, 30) },
            { Character.Daisy, new(157, 6, 31, 31) },
            { Character.DiddyKong, new(190, 7, 24, 30) },
            { Character.DonkeyKong, new(217, 6, 20, 31) },
        };
    }
}
