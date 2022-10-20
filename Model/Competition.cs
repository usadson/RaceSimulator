using System.Diagnostics.CodeAnalysis;

namespace Model
{ 
    public class Competition
    {
        public List<IParticipant> Participants { get; set; } = new();
        public Queue<Track> Tracks { get; set; } = new();
        public int TrackCount { get; set; }

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
