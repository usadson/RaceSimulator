using Model;
using System.Diagnostics.CodeAnalysis;

namespace Controller
{
    public static class TrackRegistry
    {
        public static List<Track> All { get; } = new();
        public static Dictionary<Cup, List<Track>> TracksByCup { get; } = new();
        public static Dictionary<string, Track> TrackByName { get; } = new();

        public static void Initialize()
        {
            Register(new Track(Cup.Mushroom, "Luigi Circuit", new SectionTypes[]{ 
                // nord
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.RightCorner,
                // east
                
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.RightCorner,
                // south
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.RightCorner,
                //west
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.RightCorner,
                // nord
                
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.Straight,

                //SectionTypes.Finish,
                
            }));
            Register(new Track(Cup.Mushroom, "Moo Moo Meadows", Array.Empty<SectionTypes>()));
            Register(new Track(Cup.Mushroom, "Mushroom Gorge", Array.Empty<SectionTypes>()));
            Register(new Track(Cup.Mushroom, "Toads Factory", Array.Empty<SectionTypes>()));
        }

        public static void Reset()
        {
            All.Clear();
            TracksByCup.Clear();
            TrackByName.Clear();
        }

        public static void Register([DisallowNull] Track track)
        {
            Console.WriteLine($"Adding {track.Name} with others: {All.Count}");
            All.Add(track);

            if (TracksByCup.ContainsKey(track.Cup))
                TracksByCup[track.Cup].Add(track);
            else
                TracksByCup.Add(track.Cup, new List<Track> { track });

            TrackByName.Add(track.Name, track);
        }
    }
}