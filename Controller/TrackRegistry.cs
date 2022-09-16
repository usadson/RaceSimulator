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
            Register(new Track(Cup.Mushroom, "Luigi Circuit", Array.Empty<Section>()));
            Register(new Track(Cup.Mushroom, "Moo Moo Meadows", Array.Empty<Section>()));
            Register(new Track(Cup.Mushroom, "Mushroom Gorge", Array.Empty<Section>()));
            Register(new Track(Cup.Mushroom, "Toads Factory", Array.Empty<Section>()));
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