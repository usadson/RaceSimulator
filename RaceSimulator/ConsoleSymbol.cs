using Model;
using System.Diagnostics;

namespace RaceSimulator;

public class ConsoleSymbol
{
    public static readonly ConsoleSymbol StraightEast = new(new[]
    {
        "─",
        "2",
        "1",
        "─"
    });

    public static readonly ConsoleSymbol StraightWest = new(new[]
    {
        "─",
        "1",
        "2",
        "─"
    });

    public static readonly ConsoleSymbol StraightNorth = new(new[]
    {
        "│2  1│"
    });
    public static readonly ConsoleSymbol StraightSouth = new(new[]
    {
        "│1  2│"
    });

    public static readonly ConsoleSymbol RightCornerNorth = new(new[]
    {
        "┌─────",
        "│ 2   ",
        "│   1 ",
        "│    ┌",
    });
    public static readonly ConsoleSymbol RightCornerEast = new(new[]
    {
        "─────┐",
        "   2 │",
        " 1   │",
        "┐    │",
    });
    public static readonly ConsoleSymbol RightCornerSouth = new(new[]
    {
        "┘    │",
        " 1   │",
        "   2 │",
        "─────┘",
    });
    public static readonly ConsoleSymbol RightCornerWest = new(new[]
    {
        "|    └",
        "│   1 ",
        "| 2   ",
        "└─────",
    });

    public static readonly ConsoleSymbol FinishHorizontal = new(new[]
    {
        "─┰─",
        " ⁞ ",
        " ⁞ ",
        "─┸─"
    });

    public static readonly ConsoleSymbol FinishVertical = new(new[]
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

    public static ConsoleSymbol? FindSymbol(SectionTypes sectionType, Direction direction)
    {
        if (!Symbols.TryGetValue(sectionType, out var dict)) return null;
        return dict.TryGetValue(direction, out var symbol) ? symbol : null;
    }

    public static Bounds CalculateDimensionsOfTrack(Track track)
    {
        int x = 0;
        int y = 0;

        int maxX = x;
        int maxY = y;

        int minX = 0;
        int minY = 0;

        var direction = track.BeginDirection;

        var it = track.Sections.First;
        while (it != null)
        {
            var section = it.Value;
            it = it.Next;
            var nextSection = it?.Value;

            var symbol = FindSymbol(section.SectionType, direction);

            Debug.Assert(symbol != null);

            var outerX = x + symbol.Width;
            var outerY = y + symbol.Height;
            Debug.WriteLine($"x={x} outerX={outerX}     y={y} outerY={outerY} minY={minY}");

            if (maxX < outerX)
                maxX = outerX;
            if (maxY < outerY)
                maxY = outerY;

            if (minX > x)
                minX = x;
            if (minY > y)
                minY = y;

            switch (section.SectionType)
            {
                case SectionTypes.LeftCorner:
                    direction = Directions.RotateLeft(direction);
                    break;
                case SectionTypes.RightCorner:
                    direction = Directions.RotateRight(direction);
                    break;
            }

            if (nextSection == null)
                continue;

            var nextSymbol = ConsoleSymbol.FindSymbol(nextSection.SectionType, direction);
            Debug.Assert(nextSymbol != null);

            switch (direction)
            {
                case Direction.North:
                    y -= nextSymbol.Height;
                    break;
                case Direction.South:
                    y += symbol.Height;
                    break;
                case Direction.East:
                    x += symbol.Width;
                    break;
                case Direction.West:
                    x -= nextSymbol.Width;
                    break;
            }
        }

        Debug.WriteLine($"minX={minX} maxX={maxX}");
        Debug.WriteLine($"minY={minY} maxY={maxY}");
        return new Bounds(minX, minY, maxX, maxY);
    }

    public string[] Data { get; init; }

    public int Width => Data[0].Length;
    public int Height => Data.Length;

    public readonly (int X, int Y) LeftPosition;
    public readonly (int X, int Y) RightPosition;

    public ConsoleSymbol(string[] data)
    {
        Debug.Assert(data.Length > 0, "Empty is data not allowed");
        Debug.Assert(data[0].Length > 0, "Empty strings aren't allowed");
        Debug.Assert(data.Select(str => str.Length).Distinct().Count() == 1, "Strings aren't all the same length");
        Data = data;

        for (int y = 0; y < data.Length; ++y)
        {
            var left = data[y].IndexOf('1');
            if (left != -1)
                LeftPosition = (left, y);

            var right = data[y].IndexOf('2');
            if (right != -1)
                RightPosition = (right, y);
        }
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