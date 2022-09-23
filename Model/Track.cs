using System.Diagnostics.CodeAnalysis;

namespace Model
{
    public class Track
    {
        public Cup Cup { get; }
        public string Name { get; }
        public LinkedList<Section> Sections { get; }

        public Track([DisallowNull] Cup cup, [DisallowNull] string name, [DisallowNull] SectionTypes[] sectionTypes)
        {
            Cup = cup;
            Name = name;
            Sections = ConvertSectionTypesToSection(sectionTypes);
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
