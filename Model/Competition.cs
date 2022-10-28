using System.Diagnostics.CodeAnalysis;

namespace Model
{ 
    public class Competition
    {
        public Cup Cup { get; }
        public List<IParticipant> Participants { get; set; } = new();
        public Queue<Track> Tracks { get; set; } = new();
        public int TrackCount { get; set; }

        public Competition(Cup cup)
        {
            Cup = cup;
        }

        public Track? NextTrack()
        {
            if (TrackCount == 0)
                TrackCount = Tracks.Count;
            if (Tracks.TryDequeue(out Track? track))
                return track;
            return null;
        }
    }
}
