using Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceSimulator
{
    public abstract class Application
    {
        public abstract void Run();

        public void InitializeData()
        {
            Data.NextRace();
            Debug.Assert(Data.CurrentCompetition != null);
        }

        public virtual void Update()
        {
            //Data.CurrentRace.Track.Sections.
        }
    }
}
