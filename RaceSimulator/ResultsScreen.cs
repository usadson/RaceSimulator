using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace RaceSimulator;

public sealed class ResultsScreen : Screen
{
    private List<IParticipant> _participants;

    public ResultsScreen([DisallowNull] List<IParticipant> participants)
    {
        _participants = participants;
    }

    public override void Draw(int startY)
    {
        var strings = new string[_participants.Count];
        var times = new string[strings.Length];
        int maxLength = 0;
        foreach (var participant in _participants)
        {
            Debug.Assert(participant.Ranking != null);
            var index = (int)participant.Ranking - 1;
            var line = string.Format(
                $"{participant.Ranking}   {I18N.Translate(participant.Name)}"
            );

            strings[index] = line;
            times[index] = participant.Time.Value.TotalSeconds.ToString("N2");

            if (maxLength < line.Length)
                maxLength = line.Length;
        }

        var maxLengthBeforeTime = maxLength;
        for (int i = 0; i < strings.Length; ++i)
        {
            var line = strings[i];
            strings[i] = string.Format($"{line} {new string(' ', maxLengthBeforeTime - line.Length)} {times[i]}");

            if (maxLength < line.Length)
                maxLength = line.Length;
        }

        startY += (Console.WindowHeight - startY - strings.Length) / 2;

        foreach (var line in strings)
        {
            Console.SetCursorPosition((Console.WindowWidth - maxLength) / 2, startY);
            Console.Write(line);

            startY++;
        }
    }
}
