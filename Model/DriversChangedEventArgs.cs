namespace Model;

public class DriversChangedEventArgs : EventArgs
{
    public Track Track { get; init; }

    public DriversChangedEventArgs(Track track)
    {
        Track = track;
    }
}