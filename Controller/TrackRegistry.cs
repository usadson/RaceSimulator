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
            Register(new Track(Cup.Mushroom, "Luigi Circuit", 
                new TrackSectionsBuilder(Direction.North)
                        .Turn(Direction.East)
                        .GoStraight(14)
                        .AddStart()
                        .AddStart()
                        .AddStart()
                        .AddStart()
                        .Finish()
                        .GoStraight(19)
                        .Turn(Direction.South)
                        .GoStraight(10)
                        .Turn(Direction.West)
                        .GoStraight(40)
                        .Turn(Direction.North)
                        .GoStraight(10)
                        .Build(),
                Direction.North, true));
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