using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class IParticipantCompetitionComparer : IComparer<IParticipant>
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

            return (int)x.OverallRanking - (int)y.OverallRanking;
        }
    }
}
