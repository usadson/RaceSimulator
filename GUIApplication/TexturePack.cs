using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace GUIApplication;

public class TexturePack
{
    private readonly Bitmap _emptyBitmap = new(1, 1);

    public Bitmap LoadFinish(Direction direction)
    {
        if (direction is Direction.North or Direction.South)
            return SpriteManager.Load(@".\Assets\FinishHorizontal.png") ?? _emptyBitmap;
        return SpriteManager.Load(@".\Assets\FinishVertical.png") ?? _emptyBitmap;
    }

    public Bitmap Load(SectionTypes sectionType, Direction direction, string layer)
    {
        string dir = direction.ToString();
        string section = sectionType.ToString();

        if (sectionType is SectionTypes.StartGrid or SectionTypes.Finish)
            section = SectionTypes.Straight.ToString();

        if (section == "Straight")
        {
            dir = direction is Direction.North or Direction.South 
                ? "Horizontal" : "Vertical";
        }

        return SpriteManager.Load(@$".\Assets\Road_{dir}_{section}_{layer}.png") ?? _emptyBitmap;
    }

    public RectangleF GetBitmapSubsection(SectionTypes sectionTypes, Direction direction, Bitmap bitmap)
    {
        var size = bitmap.Size;
#if no
        if (direction == Direction.North || direction == Direction.South)
            size = new(size.Height, size.Width);
#endif
        return new(new Point(0, 0), size);
    }

    public PointF GetOffsetDirectionChange(SectionTypes sectionType, Direction direction)
    {
        if (sectionType == SectionTypes.RightCorner)
        {
            switch (direction)
            {
                case Direction.West:
                    return new(-43, 0); // rechtsonder
                case Direction.East:
                    return new(0, 0); // linksboven

                case Direction.North:
                    return new(0, 43); // linksonder
                case Direction.South:
                    return new(43, 0); // rechtsboven
            }
        }
        return new(0, 0);
    }

    public int DirectionToDegrees(SectionTypes sectionType, Direction direction)
    {
        return 0;
    }
}
