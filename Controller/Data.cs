using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Model;

namespace Controller
{
    public static class Data
    {
        public static Competition? CurrentCompetition { get; private set; }
        public static uint RaceInCompetition { get; private set; }

        private static Race? _currentRaceImpl = null;
        public static Race CurrentRace { 
            get {
                if (_currentRaceImpl == null)
                    throw new NullReferenceException();
                return _currentRaceImpl;
            }
            private set => _currentRaceImpl = value; 
        }

        public static bool HasRace() => _currentRaceImpl != null;
        public static bool HasNextRace => CurrentCompetition != null && CurrentCompetition.Tracks.Count > 0;

        public static void Initialize()
        {
            CurrentCompetition = new Competition();
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

            for (int i = 0; i < 8; ++i)
            {
                CurrentCompetition.Participants.Add(
                    new Driver(
                        (Character)i,
                        100,
                        new Car(),
                        TeamColors.Blue
                    )
                );
            }
        }

        private static void AssignCupToCompetition([DisallowNull] Cup cup)
        {
            Debug.Assert(CurrentCompetition != null);
            CurrentCompetition.Tracks = new(TrackRegistry.TracksByCup[cup]);
        }

        public static void PopulateTracks()
        {
            TrackRegistry.Initialize();
            AssignCupToCompetition(Cup.Mushroom);
        }

        public static void NextRace()
        {
            Debug.Assert(CurrentCompetition != null);
            Track? track = CurrentCompetition.NextTrack();

            if (track != null)
            {
                ++RaceInCompetition;
                CurrentRace = new Race(track, CurrentCompetition.Participants);
            }
            else
            {
                _currentRaceImpl = null;
            }
        }
    }
}
