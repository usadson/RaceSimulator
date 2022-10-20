using System.Diagnostics;
using Controller;
using Model;

namespace RaceSimulator;

public sealed class ResultsScreen : Screen
{
    private readonly string[] _strings;
    private readonly string[] _times;
    private readonly int _maxLength;

    private readonly string _title;
    private readonly string _nextRaceText;

    private bool _invalidated = true;

    public ResultsScreen(List<IParticipant> participants)
    {
        _strings = new string[participants.Count];
        _times = new string[_strings.Length];

        _title = I18N.Translate("ResultsScreenTitle");
        _nextRaceText = I18N.Translate("ResultsScreenNextRace")
            .Replace("TRACK_NAME", I18N.Translate(Data.CurrentCompetition.Tracks.Peek().Name))
            .Replace("INDEX", (Data.RaceInCompetition + 1).ToString())
            .Replace("COUNT", Data.CurrentCompetition.TrackCount.ToString());

        foreach (var participant in participants)
        {
            Debug.Assert(participant.Ranking != null);
            var index = (int)participant.Ranking - 1;
            var line = string.Format(
                $"{participant.Ranking}   {I18N.Translate(participant.Name)}"
            );

            _strings[index] = line;
            Debug.Assert(participant.Time != null, "participant.Time != null");
            _times[index] = participant.Time.Value.TotalSeconds.ToString("N2");

            if (_maxLength < line.Length)
                _maxLength = line.Length;
        }

        var maxLengthBeforeTime = _maxLength;
        for (int i = 0; i < _strings.Length; ++i)
        {
            var line = _strings[i];
            _strings[i] = string.Format($"{line} {new string(' ', maxLengthBeforeTime - line.Length)} {_times[i]}");

            if (_maxLength < line.Length)
                _maxLength = line.Length;
        }
    }

    public override void Draw(int startY)
    {
        if (!_invalidated)
            return;
        _invalidated = false;
        Console.Clear();

        startY += (Console.WindowHeight - startY - _strings.Length) / 2;

        var titleHorizontalPadding = (Console.WindowWidth - _title.Length) / 2;
        if (startY < 4)
        {
            Console.SetCursorPosition(titleHorizontalPadding, 0);
            startY = 2;
        }
        else
        {
            Console.SetCursorPosition(titleHorizontalPadding, startY - 3);
        }
        Console.Write(_title);

        foreach (var line in _strings)
        {
            Console.SetCursorPosition((Console.WindowWidth - _maxLength) / 2, startY);
            Console.Write(line);

            startY++;
        }

        Console.Write("\n\n");
        Console.Write(new string(' ', (Console.WindowWidth - _nextRaceText.Length) / 2));
        Console.Write(_nextRaceText);

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
    }
}
