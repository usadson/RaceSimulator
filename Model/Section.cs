using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Section
    {
        public Track Parent { get; init; }
        public SectionTypes SectionType { get; set; }
    }
}
