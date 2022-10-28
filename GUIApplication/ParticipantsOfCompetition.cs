using Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIApplication;

public sealed class ParticipantsOfCompetition : ParticipantsStatisticsView
{
    public string CompetitionName { get; }

    public ParticipantsOfCompetition()
    {
        Debug.Assert(Data.CurrentCompetition != null, "Data.CurrentCompetition != null");
        CompetitionName = I18N.Translate(Data.CurrentCompetition.Cup.ToString());
        Data.NewRaceStarting += race =>
        {
            race.ParticipantsOrderModified += UpdateParticipants;

            UpdateParticipants(race);
        };
    }

    protected override void OnDispatcherPublished()
    {
        if (Data.HasRace())
            UpdateParticipants(Data.CurrentRace);
    }

    private void UpdateParticipants(Race race)
    {
        if (Data.CurrentCompetition != null)
            UpdateParticipants(Data.CurrentCompetition.Participants, true);
    }
}