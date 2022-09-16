namespace Model
{
    public class Track
    {
        public Cup Cup { get; }
        public string Name { get; }
        public LinkedList<Section> Sections { get; }

        public Track(Cup cup, string name, Section[] sections)
        {
            Cup = cup;
            Name = name;
            Sections = new(sections);
        }
    }
}
