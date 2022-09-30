using System.Diagnostics.CodeAnalysis;

namespace Model
{
    public class Track
    {
        public Cup Cup { get; }
        public string Name { get; }
        public LinkedList<Section> Sections { get; }
        public Direction BeginDirection { get; }

        public bool IsCentered { get;  }
        public int XOffset { get; }
        public int YOffset { get; }

        public Track([DisallowNull] Cup cup, [DisallowNull] string name, [DisallowNull] SectionTypes[] sectionTypes,
            Direction beginDirection = Direction.North, bool center = true, int xOffset = 0, int yOffset = 0)
        {
            Cup = cup;
            Name = name;
            Sections = ConvertSectionTypesToSection(sectionTypes);
            BeginDirection = beginDirection;

            IsCentered = center;
            XOffset = xOffset;
            YOffset = yOffset;
        }

        private static LinkedList<Section> ConvertSectionTypesToSection([DisallowNull] SectionTypes[] sectionTypes)
        {
            return new(
                sectionTypes.Select(sectionType => new Section
                {
                    SectionType = sectionType
                })
            );
        }
    }
}
