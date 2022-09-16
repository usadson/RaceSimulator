namespace Model
{
    public class Track
    {
        public Cup Cup { get; set; }
        public string Name { get; set; }
        public LinkedList<Section> Sections;

        public Track(Cup cup, string name, Section[] sections)
        {
            Cup = cup;
            Name = name;
            Sections = new(sections);
        }
    }
}
