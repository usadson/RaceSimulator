using System.Drawing;

namespace CommonViewLibTest;

internal class MockTrackRenderer : AbstractTrackRenderer
{
    public SizeF SectionSize = new(0, 0);
    public uint GetProtected_CurrentSectionIndex => CurrentSectionIndex;
    public void MockStartDrawTrack() => StartDrawTrack();

    private (Direction, Direction)? _lastDirectionUpdate;

    public (Direction?, Direction?)? GetLastDirectionUpdate()
    {
        return _lastDirectionUpdate;
    }

    public void SetPoints(PointF leftTop, PointF rightBottom)
    {
        TopLeftmostPoint = leftTop;
        BottomRightmostPoint = rightBottom;
    }

    protected override void DirectionChanged(Direction oldDirection, Direction newDirection, SectionTypes section)
    {
        _lastDirectionUpdate = new(oldDirection, newDirection);
    }

    protected override SizeF GetSizeOfSection(SectionTypes sectionType, Direction direction)
    {
        return SectionSize;
    }

    protected override void DrawSection(Direction direction, Section section, SectionData sectionData)
    {
    }
}