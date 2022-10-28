using Controller;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RaceSimulator;

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
    }

    [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
    protected static extern bool ShouldSystemUseDarkMode();
}