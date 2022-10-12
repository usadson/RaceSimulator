using System.Diagnostics.CodeAnalysis;

namespace Model
{
    public class DriversChangedEventArgs : EventArgs
    {
        public Track Track { get; init; }

        public DriversChangedEventArgs([DisallowNull] Track track)
        {
            Track = track;
        }
    }
}
