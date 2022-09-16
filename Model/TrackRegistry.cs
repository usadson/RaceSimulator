using Model;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Controller
{
    public class TrackRegistry
    {
        public static List<Track> All { get; private set; } = new();
        public static Dictionary<Cup, List<Track>> TrackByCup { get; private set; } = new();
        public static Dictionary<string, Track> TrackByName { get; private set; } = new();

        public static void Initialize()
        {
            Register(new Track(Cup.Mushroom, "Luigi Circuit", Array.Empty<Section>()));
            Register(new Track(Cup.Mushroom, "Moo Moo Meadows", Array.Empty<Section>()));
            Register(new Track(Cup.Mushroom, "Mushroom Gorge", Array.Empty<Section>()));
            Register(new Track(Cup.Mushroom, "Toads Factory", Array.Empty<Section>()));
        }

        public static void Register([DisallowNull] Track track)
        {
            All.Add(track);

            if (TrackByCup.ContainsKey(track.Cup))
                TrackByCup[track.Cup].Add(track);
            else
                TrackByCup.Add(track.Cup, new List<Track> { track });

            TrackByName.Add(track.Name, track);
        }
    }
}
