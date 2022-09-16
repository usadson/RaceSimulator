using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Model;

namespace Controller
{
    public static class Data
    {
        public static Competition? CurrentCompetition { get; private set; }
        public static Race? CurrentRace { get; private set; }

        public static void Initialize()
        {
            CurrentCompetition = new Competition();
            PopulateParticipants();
            PopulateTracks();
        }

        public static void Reset()
        {
            CurrentCompetition = null;
            CurrentRace = null;
            TrackRegistry.Reset();
        }

        public static void PopulateParticipants()
        {
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
            CurrentCompetition.Tracks = new(TrackRegistry.TracksByCup[cup]);
        }

        public static void PopulateTracks()
        {
            TrackRegistry.Initialize();
            AssignCupToCompetition(Cup.Mushroom);
        }

        public static void NextRace()
        {
            Track? track = CurrentCompetition.NextTrack();

            if (track != null)
            {
                CurrentRace = new Race(track, CurrentCompetition.Participants);
            }
        }
    }
}
