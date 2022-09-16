using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    internal class Track
    {
        public string Name { get; set; }
        public LinkedList<Section> Sections;

        public Track(string name, Section[] sections)
        {
            Name = name;
            Sections = new(sections);
        }
    }
}
