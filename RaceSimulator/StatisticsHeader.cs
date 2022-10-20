using System.Diagnostics;
using Controller;
using Model;

namespace RaceSimulator;

public sealed class StatisticsHeader
{
    public readonly int Height = 6;

    public bool IsInvalidated { get; set; }

    public void Draw(bool darkMode)
    {
        IsInvalidated = false;
        Console.SetCursorPosition(Console.WindowLeft, Console.WindowTop);
        Console.WriteLine();

        Console.ForegroundColor = darkMode ? ConsoleColor.Yellow : ConsoleColor.DarkYellow;
        Console.Write(I18N.Translate("HeaderRaceTrackLabel"));
        Console.ForegroundColor = darkMode ? ConsoleColor.Gray : ConsoleColor.Black;
        lock (Data.CurrentRace)
            Console.Write(I18N.Translate(Data.CurrentRace.Track.Name));
        FillLineTillEnd();

        Console.ForegroundColor = darkMode ? ConsoleColor.Yellow : ConsoleColor.DarkYellow;
        Console.Write(I18N.Translate("HeaderParticipantsLabel"));
        Console.ForegroundColor = darkMode ? ConsoleColor.Gray : ConsoleColor.Black;
        lock (Data.CurrentRace)
        {
            Console.Write(
                string.Join(", ",
                    Data.CurrentRace.Participants.Select(
                        participant => string.Format($"{I18N.Translate(((Driver)participant).Character.ToString())} {participant.LapStringified}")
                    )));
        }
        FillLineTillEnd();
        FillLineTillEnd();
        FillLineTillEnd('=');
        
        Debug.Assert(Height >= Console.GetCursorPosition().Top);
    }

    private static void FillLineTillEnd(char character = ' ', bool line = true, int paddingRight = 0)
    {
        Console.Write(new string(character, Console.WindowWidth - Console.CursorLeft - paddingRight));
        if (line)
            Console.WriteLine();
    }
}
