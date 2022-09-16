using System.Diagnostics.CodeAnalysis;

namespace Model
{ 
    public class Competition
    {
        public List<IParticipant> Participants { get; set; } = new();
        public Queue<Track> Tracks { get; set; } = new();

        [return: MaybeNull]
        public Track? NextTrack()
        {
            if (Tracks.TryDequeue(out Track? track))
                return track;
            return null;
        }
    }
}
