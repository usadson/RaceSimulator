using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Controller;
using Model;

namespace RaceSimulator;

public class CompetitionFinishedScreen : Screen
{
    private bool _invalidated = true;

    private readonly string[] _congratsText;
    private readonly string[] _namesText;
    private readonly string[] _resultsText;

    private readonly int _longestLineLength;
    private readonly int _longestParticipantNameLength;

    public CompetitionFinishedScreen()
    {
        Data.CurrentRace.Participants.Sort(new IParticipantCompetitionComparer());

        var winner = Data.CurrentRace.Participants.First();
        _congratsText = I18N.Translate("CongratulationsParticipantWon")
                .Replace("PARTICIPANT", I18N.Translate(winner.Name))
                .Split('\n');

        _namesText = Data.CurrentRace.Participants.Select(participant => I18N.Translate(participant.Name)).ToArray();
        _resultsText = Data.CurrentRace.Participants.Select(participant => participant.OverallRanking.ToString()).ToArray();
        
        foreach (var line in _congratsText)
            if (_longestLineLength < line.Length)
                _longestLineLength = line.Length;

        foreach (var line in _namesText)
            if (_longestParticipantNameLength < line.Length)
                _longestParticipantNameLength = line.Length;

        foreach (var line in _resultsText)
        {
            var length = _longestParticipantNameLength + 2 + line.Length;
            if (_longestLineLength < length)
                _longestLineLength = length;
        }
    }

    public override void Draw(int startY)
    {
        if (!_invalidated)
            return;
        _invalidated = false;

        Console.Clear();
        Console.SetCursorPosition(0, 0);

        var startX = (Console.WindowWidth - _longestParticipantNameLength) / 2;

        Console.WriteLine("\n\n");
        foreach (var line in _congratsText)
        {
            Console.CursorLeft = (Console.WindowWidth - line.Length) / 2;
            Console.WriteLine(line);
        }

        Console.WriteLine("\n\n");

        for (int i = 0; i < _namesText.Length; ++i)
        {
            Console.Write(new string(' ', startX));
            Console.Write(_namesText[i]);
            
            Console.SetCursorPosition(startX + _longestParticipantNameLength, Console.CursorTop);
            Console.WriteLine(_resultsText[i]);
        }
    }

    public override void OnResize(Bounds currentBounds)
    {
        _invalidated = true;
    }

    public override void OnTrackInvalidate()
    {
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
