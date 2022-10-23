using Model;

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
                        .GoStraight(12)
                        .AddStart()
                        .AddStart()
                        .AddStart()
                        .AddStart()
                        .AddStart()
                        .AddStart()
                        .Finish()
                        .GoStraight(19)
                        .Turn(Direction.South)
                        .GoStraight(10)
                        .Turn(Direction.West)
                        .GoStraight(38)
                        .Turn(Direction.North)
                        .GoStraight(10)
                        .Build(),
                3, Direction.North, true));
            Register(new Track(
                Cup.Mushroom, 
                "Moo Moo Meadows", 
                new TrackSectionsBuilder(Direction.North)
                    .AddStart()
                    .AddStart()
                    .AddStart()
                    .AddStart()
                    .AddStart()
                    .AddStart()
                    .Finish()
                    .GoStraight(3)
                    .Turn(Direction.East)
                    .GoStraight(60)
                    .Turn(Direction.South)
                    .GoStraight(10)
                    .Turn(Direction.West)
                    .GoStraight(60)
                    .Turn(Direction.North)
                    .Build(),
                3
            ));
            Register(new Track(
                Cup.Mushroom, 
                "Mushroom Gorge",
                new TrackSectionsBuilder(Direction.North)
                    .Turn(Direction.East)
                    .GoStraight(12)
                    .AddStart()
                    .AddStart()
                    .AddStart()
                    .AddStart()
                    .AddStart()
                    .AddStart()
                    .Finish()
                    .GoStraight(19)
                    .Turn(Direction.South)
                    .GoStraight(1)
                    .Turn(Direction.West)
                    .GoStraight(30)
                    .Turn(Direction.South)
                    .GoStraight(1)
                    .Turn(Direction.East)
                    .GoStraight(30)
                    .Turn(Direction.South)
                    .GoStraight(1)
                    .Turn(Direction.West)
                    .GoStraight(40)
                    .Turn(Direction.North)
                    .GoStraight(5)
                    .Turn(Direction.East)
                    .GoStraight(10)
                    .Build(),
                3
            ));
            //Register(new Track(Cup.Mushroom, "Toads Factory", Array.Empty<SectionTypes>(), 3));
        }

        public static void Reset()
        {
            All.Clear();
            TracksByCup.Clear();
            TrackByName.Clear();
        }

        public static void Register(Track track)
        {
            All.Add(track);

            if (TracksByCup.ContainsKey(track.Cup))
                TracksByCup[track.Cup].Add(track);
            else
                TracksByCup.Add(track.Cup, new List<Track> { track });

            TrackByName.Add(track.Name, track);
        }
    }
}