using System.Diagnostics;
using Controller.Properties;
using Model;

namespace Controller
{
    public static class Data
    {
        public static Competition? CurrentCompetition { get; private set; }
        public static uint RaceInCompetition { get; private set; }

        public static double Speed = 1.0f;

        private static Race? _currentRaceImpl;
        public static Race CurrentRace { 
            get {
                if (_currentRaceImpl == null)
                    throw new NullReferenceException();
                return _currentRaceImpl;
            }
            set => _currentRaceImpl = value; 
        }

        public static bool HasRace() => _currentRaceImpl != null;
        public static bool HasNextRace => CurrentCompetition != null && CurrentCompetition.Tracks.Count > 0;

        public delegate void RaceBreakdownEventHandler(Race race);
        public delegate void NewRaceStartingEventHandler(Race race);

        public static event RaceBreakdownEventHandler? RaceBreakdown;
        public static event NewRaceStartingEventHandler? NewRaceStarting;

        public static void Initialize()
        {
            Speed = 1.0f;
            CurrentCompetition = new Competition(Cup.Mushroom);
            RaceInCompetition = 0;
            _currentRaceImpl = null;
            PopulateParticipants();
            PopulateTracks();
        }

        public static void Reset()
        {
            CurrentCompetition = null;
            _currentRaceImpl = null;
            TrackRegistry.Reset();
        }

        public static void PopulateParticipants()
        {
            Debug.Assert(CurrentCompetition != null);

            var characters = new LinkedList<Character>(Enum.GetValues<Character>());

            var random = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < 12; ++i)
            {
                var character = characters.ElementAt(random.Next(0, characters.Count));
                characters.Remove(character);

                CurrentCompetition.Participants.Add(
                    new Driver(
                        character,
                        100,
                        new Car(),
                        TeamColors.Blue
                    )
                );
            }
        }

        private static void AssignCupToCompetition()
        {
            Debug.Assert(CurrentCompetition != null);
            CurrentCompetition.Tracks = new(TrackRegistry.TracksByCup[CurrentCompetition.Cup]);
        }

        public static void PopulateTracks()
        {
            TrackRegistry.Initialize();
            AssignCupToCompetition();
        }

        public static void NextRace()
        {
            if (_currentRaceImpl != null)
            {
                RaceBreakdown?.Invoke(_currentRaceImpl);
                _currentRaceImpl.Dispose();
            }

            Debug.Assert(CurrentCompetition != null);
            Track? track = CurrentCompetition.NextTrack();

            if (track != null)
            {
                ++RaceInCompetition;

                foreach (var participant in CurrentCompetition.Participants)
                    participant.OnNewRace(CurrentCompetition);

                CurrentRace = new Race(track, CurrentCompetition.Participants);
                NewRaceStarting?.Invoke(CurrentRace);
            }
            else
            {
                _currentRaceImpl = null;
            }
        }

        public static Resources CreateResources()
        {
            return new();
        }
    }
}
