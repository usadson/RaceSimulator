using Controller;
using Model;
using System.ComponentModel;
using System.Windows;

namespace GUIApplication;

public struct ParticipantEntry : INotifyPropertyChanged
{
    public static readonly Int32Rect[] PositionRects = {
        new(31, 1, 108, 119), // 1
        new(169, 3, 112, 116), // 2
        new(319, 3, 120, 112), // 3
        new(479, 4, 122, 110), // 4
        new(31, 139, 131, 112), // 5
        new(187, 138, 121, 114), // 6
        new(325, 136, 122, 115), // 7
        new(479, 133, 128, 116), // 8
        new(1, 271, 118, 116), // 9
        new(140, 266, 168, 121), // 10
        new(313, 268, 166, 118), // 11
        new(497, 267, 166, 119) // 12
    };

    public IParticipant Participant { get; }
    public Character Character { get; }

    public string Name { get; }
    public ushort PositionInRace { get; }
    public ushort PositionInCompetition => Participant.PositionInCompetition;

    private readonly bool _competitionEntry;

    public Int32Rect PictureRect { get; }
    public Int32Rect PositionRect {
        get
        {
            var index = _competitionEntry ? PositionInCompetition - 1 : PositionInRace - 1;
            if (index >= 0 && index < PositionRects.Length)
                return PositionRects[index];
            return PositionRects[^1];
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged = null;

    public string CompetitionPoints
    {
        get
        {
            var points = Participant.CompetitionPoints;

            var raw = $"Points_{points}";
            string result = I18N.Translate(raw);
            if (result != raw)
                return result;

            return string.Format(I18N.Translate("PointsFormatter"), points);
        }
    }

    public ParticipantEntry(IParticipant participant, bool competitionEntry)
    {
        _competitionEntry = competitionEntry;

        Name = I18N.Translate(participant.Name);
        PositionInRace = participant.PositionInRace;
        //PositionInCompetition = participant.PositionInCompetition;
        Participant = participant;
        Character = ((Driver)participant).Character;

        var rect = SpriteOffsets.MinimapOffsets[Character];
        PictureRect = new(rect.X, rect.Y, rect.Width, rect.Height);

        PropertyChanged?.Invoke(this, new(nameof(PictureRect)));
    }
}