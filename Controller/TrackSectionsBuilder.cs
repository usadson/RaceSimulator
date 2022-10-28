using Model;
using System.Diagnostics;

namespace Controller;

public class TrackSectionsBuilder
{
    private class Action
    {
        public enum Types
        {
            Normal,
            Finish,
            Start,
        }

        public Types Type = Types.Normal;
        public Direction? Dir;
        public uint Steps = 1;
    }

    private List<Action> _actions = new();
    private Direction _direction;
    private Direction _lastDirectionSet;
    private bool _finished;

    public TrackSectionsBuilder(Direction beginDirection)
    {
        _direction = beginDirection;
        _lastDirectionSet = _direction;
    }

    public TrackSectionsBuilder GoStraight(uint steps)
    {
        _actions.Add(new Action
        {
            Steps = steps
        });

        return this;
    }

    public TrackSectionsBuilder AddStart()
    {
        _actions.Add(new Action
        {
            Type = Action.Types.Start
        });

        return this;
    }

    public TrackSectionsBuilder Finish()
    {
        Debug.Assert(!_finished, "There can only be one finish.");
        _finished = true;

        _actions.Add(new Action
        {
            Type = Action.Types.Finish
        });

        return this;
    }

    public TrackSectionsBuilder Turn(Direction direction)
    {
        if (_lastDirectionSet == direction)
            throw new InvalidOperationException("Turning in the direction the track was already moving on");

        switch (_lastDirectionSet)
        {
            case Direction.North:
            case Direction.South:
                Debug.Assert(direction is Direction.East or Direction.West);
                break;
            case Direction.East:
            case Direction.West:
                Debug.Assert(direction is Direction.North or Direction.South);
                break;
        }

        _actions.Add(new Action
        {
            Dir = direction
        });

        _lastDirectionSet = direction;
        return this;
    }

    public SectionTypes[] Build()
    {
        Debug.Assert(_finished, "There should be a finish line");

        uint size = 0;
        foreach (var action in _actions)
            size += action.Steps;

        var types = new SectionTypes[size];
        var position = 0;
        foreach (var action in _actions)
        {
            if (action.Dir == null)
            {
                switch (action.Type)
                {
                    case Action.Types.Normal:
                        for (int i = 0; i < action.Steps; ++i)
                            types[position++] = SectionTypes.Straight;
                        break;
                    case Action.Types.Finish:
                        types[position++] = SectionTypes.Finish;
                        break;
                    case Action.Types.Start:
                        types[position++] = SectionTypes.StartGrid;
                        break;
                }
                    
            }
            else
            {
                types[position++] = CalculateSectionType(_direction, (Direction)action.Dir);

                _direction = (Direction)action.Dir;
            } 
        }

        return types;
    }

    protected static SectionTypes CalculateSectionType(Direction from, Direction to)
    {
        switch (from)
        {
            case Direction.North:
                if (to == Direction.East)
                    return SectionTypes.RightCorner;
                    
                Debug.Assert(to == Direction.West);
                return SectionTypes.LeftCorner;
            case Direction.South:
                if (to == Direction.East)
                    return SectionTypes.LeftCorner;
                    
                Debug.Assert(to == Direction.West);
                return SectionTypes.RightCorner;
            case Direction.East:
                if (to == Direction.North)
                    return SectionTypes.LeftCorner;
                    
                Debug.Assert(to == Direction.South);
                return SectionTypes.RightCorner;
            case Direction.West:
                if (to == Direction.North)
                    return SectionTypes.RightCorner;
                    
                Debug.Assert(to == Direction.South);
                return SectionTypes.LeftCorner;
        }

        throw new InvalidDataException("Invalid data");
    }
}