using Controller;

namespace GUIApplication;

public sealed class ParticipantsOfRace : ParticipantsStatisticsView
{
    public string TrackName => Data.HasRace() ? I18N.Translate(Data.CurrentRace.Track.Name) : "TrackName";
    public string NextTrackName => Data.HasNextRace ? I18N.Translate(Data.CurrentCompetition?.Tracks.Peek().Name ?? "") : "";

    public ParticipantsOfRace()
    {
        Data.NewRaceStarting += race => { 
            PropertyChangedManually(nameof(TrackName));

            race.ParticipantsOrderModified += UpdateParticipants;

            UpdateParticipants(race);
        };
    }

    protected override void OnDispatcherPublished()
    {
        if (Data.HasRace())
            UpdateParticipants(Data.CurrentRace);
    }

    private void UpdateParticipants(Race race) => UpdateParticipants(race.Participants, false);
}

