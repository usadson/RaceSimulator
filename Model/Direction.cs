using System.ComponentModel;

namespace Model;

public enum Direction
{
    North,
    East,
    South,
    West
}

public static class Directions
{
    public static Direction RotateLeft(Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.West,
            Direction.East => Direction.North,
            Direction.South => Direction.East,
            Direction.West => Direction.South,
            _ => throw new InvalidEnumArgumentException(),
        };
    }

    public static Direction RotateRight(Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.East,
            Direction.East => Direction.South,
            Direction.South => Direction.West,
            Direction.West => Direction.North,
            _ => throw new InvalidEnumArgumentException(),
        };
    }

    public static Direction Opposite(Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.South,
            Direction.East => Direction.West,
            Direction.South => Direction.North,
            Direction.West => Direction.East,
            _ => throw new InvalidEnumArgumentException(),
        };
    }
}
