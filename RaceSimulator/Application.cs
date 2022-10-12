using Controller;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        protected static extern bool ShouldSystemUseDarkMode();
    }
}
