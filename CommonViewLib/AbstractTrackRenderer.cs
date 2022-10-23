using System.Drawing;
using Controller;
using Model;

namespace CommonViewLib;

public abstract class AbstractTrackRenderer
{
    public float X { get; set; }
    public float Y { get; set; }
    protected uint CurrentSectionIndex { get; private set; }

    public PointF CurrentPoint => new(X, Y);

    public PointF TopLeftmostPoint { get; protected set; }
    public PointF BottomRightmostPoint { get; protected set; }

    private Direction _direction;

    protected abstract void DirectionChanged(Direction oldDirection, Direction newDirection, SectionTypes section);
    protected abstract SizeF GetSizeOfSection(SectionTypes sectionType, Direction direction);
    protected abstract void DrawSection(Direction direction, Section section, SectionData sectionData);

    protected void StartDrawTrack()
    {
        _direction = Data.CurrentRace.Track.BeginDirection;
        CurrentSectionIndex = 0;

        var sections = Data.CurrentRace.Track.Sections;
        var it = sections.First;
        while (it != null)
        {
            DrawSection(it.Value, it.Next?.Value, Data.CurrentRace.GetSectionData(it.Value));

            it = it.Next;
            ++CurrentSectionIndex;
        }
    }

    private void DrawSection(Section section, Section? nextSection, SectionData data)
    {
        DrawSection(_direction, section, data);
        UpdateDirection(section.SectionType);
        MoveCursor(section, nextSection);
    }

    private void UpdateDirection(SectionTypes sectionType)
    {
        var oldDirection = _direction;
        switch (sectionType)
        {
            case SectionTypes.LeftCorner:
                _direction = Directions.RotateLeft(_direction);
                break;
            case SectionTypes.RightCorner:
                _direction = Directions.RotateRight(_direction);
                break;
            default:
                return;
        }

        DirectionChanged(oldDirection, _direction, sectionType);
    }

    private void MoveCursor(Section section, Section? nextSection)
    {
        var symbolSize = GetSizeOfSection(section.SectionType, _direction);
        SizeF? nextSymbolSize = nextSection != null ? GetSizeOfSection(nextSection.SectionType, _direction) : null;

        UpdateOutermostPoints();

        switch (_direction)
        {
            case Direction.North:
                if (nextSymbolSize == null)
                    return;
                Y -= nextSymbolSize.Value.Height;
                break;
            case Direction.South:
                Y += symbolSize.Height;
                break;
            case Direction.East:
                X += symbolSize.Width;
                break;
            case Direction.West:
                if (nextSymbolSize == null)
                    return;
                X -= nextSymbolSize.Value.Width;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateOutermostPoints()
    {
        if (TopLeftmostPoint.X > X)
            TopLeftmostPoint = TopLeftmostPoint with { X = X };

        if (TopLeftmostPoint.Y > Y)
            TopLeftmostPoint = TopLeftmostPoint with { Y = Y };

        if (X > BottomRightmostPoint.X)
            BottomRightmostPoint = BottomRightmostPoint with { X = X };

        if (Y > BottomRightmostPoint.Y)
            BottomRightmostPoint = BottomRightmostPoint with { Y = Y };
    }
}
