using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceSimulator
{
    public class ConsoleSymbol
    {
        public static readonly ConsoleSymbol StraightEast = new(new string[]
        {
            "─",
            "2",
            "1",
            "─"
        });

        public static readonly ConsoleSymbol StraightWest = new(new string[]
        {
            "─",
            "1",
            "2",
            "─"
        });

        public static readonly ConsoleSymbol StraightNorth = new(new string[]
        {
            "│2  1│"
        });
        public static readonly ConsoleSymbol StraightSouth = new(new string[]
        {
            "│1  2│"
        });

        public static readonly ConsoleSymbol RightCornerNorth = new(new string[]
        {
            "┌─────",
            "│ 2   ",
            "│   1 ",
            "│    ┌",
        });
        public static readonly ConsoleSymbol RightCornerEast = new(new string[]
        {
            "─────┐",
            "   2 │",
            " 1   │",
            "┐    │",
        });
        public static readonly ConsoleSymbol RightCornerSouth = new(new string[]
        {
            "┘    │",
            " 1   │",
            "   2 │",
            "─────┘",
        });
        public static readonly ConsoleSymbol RightCornerWest = new(new string[]
        {
            "|    └",
            "│   1 ",
            "| 2   ",
            "└─────",
        });

        public static readonly ConsoleSymbol FinishHorizontal = new(new string[]
        {
            "─┰─",
            " ⁞ ",
            " ⁞ ",
            "─┸─"
        });

        public static readonly ConsoleSymbol FinishVertical = new(new string[]
        {
            "┝----┥"
        });

        public static readonly Dictionary<SectionTypes, Dictionary<Direction, ConsoleSymbol>> Symbols = new ()
        {
            {
                SectionTypes.Straight,
                new Dictionary<Direction, ConsoleSymbol>()
                {
                    { Direction.North, StraightNorth },
                    { Direction.East, StraightEast },
                    { Direction.South, StraightSouth },
                    { Direction.West, StraightWest },
                }
            },
            {
                SectionTypes.LeftCorner,
                new Dictionary<Direction, ConsoleSymbol>()
                {
                    { Direction.North, RightCornerSouth },
                    { Direction.East, RightCornerWest },
                    { Direction.South, RightCornerNorth },
                    { Direction.West, RightCornerEast },
                }
            },
            {
                SectionTypes.RightCorner,
                new Dictionary<Direction, ConsoleSymbol>()
                {
                    { Direction.North, RightCornerNorth },
                    { Direction.East, RightCornerEast },
                    { Direction.South, RightCornerSouth },
                    { Direction.West, RightCornerWest },
                }
            },
            {
                SectionTypes.Finish,
                new Dictionary<Direction, ConsoleSymbol>()
                {
                    { Direction.North, FinishVertical },
                    { Direction.East, FinishHorizontal },
                    { Direction.South, FinishVertical },
                    { Direction.West, FinishHorizontal },
                }
            },
            {
                SectionTypes.StartGrid,
                new Dictionary<Direction, ConsoleSymbol>()
                {
                    { Direction.North, StraightNorth },
                    { Direction.East, StraightEast },
                    { Direction.South, StraightSouth },
                    { Direction.West, StraightWest },
                }
            },
        };

        public string[] Data { get; init; }

        public int Width => Data[0].Length;
        public int Height => Data.Length;

        public ConsoleSymbol(string[] data)
        {
            Debug.Assert(data.Length > 0, "Empty is data not allowed");
            Debug.Assert(data[0].Length > 0, "Empty strings aren't allowed");
            Debug.Assert(data.Select(str => str.Length).Distinct().Count() == 1, "Strings aren't all the same length");
            Data = data;
        }

        public void Draw(int beginX, int beginY, Bounds withinBounds, string replacement1, string replacement2)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                if (beginY + i < withinBounds.Top || beginY + i >= withinBounds.Bottom)
                    break;

                var length = Math.Min(beginX + Data[i].Length, withinBounds.Right) - beginX;

                if (beginX >= withinBounds.Left)
                {
                    Console.SetCursorPosition(beginX, beginY + i);
                    Console.Write(Data[i][..length].Replace("1", replacement1).Replace("2", replacement2));
                }
                else if (beginX + Data[i].Length > withinBounds.Left)
                {
                    Console.SetCursorPosition(withinBounds.Left, beginY + i);

                    var begin = withinBounds.Left - beginX;
                    Console.Write(Data[i][begin..length].Replace("1", replacement1).Replace("2", replacement2));
                }
            }
        }
    }
}
