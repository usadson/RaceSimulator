using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceSimulator;

public abstract class Screen : IDisposable
{
    public abstract void Draw(int startY);

    public abstract void OnResize(Bounds currentBounds);
    public abstract void OnTrackInvalidate();
    public abstract void Dispose();
}
