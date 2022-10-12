using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class IParticipantDistanceComparer : IComparer<IParticipant>
    {
        // Sort on reverse DistanceFromStart
        public int Compare(IParticipant? x, IParticipant? y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null && y != null)
                return 1;
            if (x != null && y == null)
                return -1;

            return (int)y.DistanceFromStart - (int)x.DistanceFromStart;
        }
    }
}
