using Controller;
using Model;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace RaceSimulator;

[SupportedOSPlatform("windows")]
internal sealed class TrackScreen : Screen
{
    public const string ParticipantBrokenCharacter = "Ω";

    private Direction _direction = Direction.North;
    public bool IsInvalidated { get; private set; }

    private Bounds _trackBounds = new(0, 0, 0, 0);
    private Bounds _trackSize = new(0, 0, 0, 0);

    private readonly ConsoleGraphicsRenderer _consoleGraphicsRenderer = new();

    private int _x, _y;

    public override void Draw(int startY)
    {
        //if (!IsInvalidated)
        //    return;
        IsInvalidated = false;

        Debug.Assert(_trackBounds.Width > 0);
        Debug.Assert(_trackBounds.Height > 0);

        _direction = Data.CurrentRace.Track.BeginDirection;

        if (Data.CurrentRace.Track.IsCentered)
        {
            Debug.Assert(_trackSize.MiddleX != 0);
            Debug.Assert(_trackSize.MiddleY != 0);
            _x = _trackBounds.MiddleX - _trackSize.MiddleX;
            _y = _trackBounds.MiddleY - _trackSize.MiddleY;
            if (_y + _trackSize.Height > Console.WindowHeight)
            {
                if (_trackSize.Height + startY > Console.WindowHeight)
                    _y = Console.WindowHeight - _trackSize.Height;
                else
                    _y = startY - _trackSize.MiddleY + _trackSize.Height;
            }
        }
        else
        {
            _x = _trackBounds.X + Data.CurrentRace.Track.XOffset;
            _y = _trackBounds.Y + Data.CurrentRace.Track.YOffset;
        }

        var sections = Data.CurrentRace.Track.Sections;
        var it = sections.First;
        while (it != null)
        {
            DrawSection(it.Value, it.Next?.Value, Data.CurrentRace.GetSectionData(it.Value));

            it = it.Next;
        }
    }

    private void DrawSection(Section section, Section? nextSection, SectionData data)
    {
        var symbol = ConsoleSymbol.FindSymbol(section.SectionType, _direction);

        if (symbol == null)
            return;

        if (data.Changed)
        {
            data.Changed = false;

            var replacement1 = " ";
            var replacement2 = " ";

            if (!_consoleGraphicsRenderer.IsReady)
            {
                if (data.Left.Participant is { Ranking: null })
                {
                    replacement1 = data.Left.Participant.Equipment.IsBroken 
                        ? ParticipantBrokenCharacter 
                        : I18N.Translate("Letter of " + ((Driver)data.Left.Participant).Character);
                }

                if (data.Right.Participant is { Ranking: null })
                {
                    replacement2 = data.Right.Participant.Equipment.IsBroken 
                        ? ParticipantBrokenCharacter 
                        : I18N.Translate("Letter of " + ((Driver)data.Right.Participant).Character);
                }
            }

            symbol.Draw(_x, _y, _trackBounds, replacement1, replacement2);
            _consoleGraphicsRenderer.DrawSection(symbol, data);
        }

        _direction = section.SectionType switch
        {
            SectionTypes.LeftCorner => Directions.RotateLeft(_direction),
            SectionTypes.RightCorner => Directions.RotateRight(_direction),
            _ => _direction
        };

        var nextSymbol = nextSection != null ? ConsoleSymbol.FindSymbol(nextSection.SectionType, _direction) : null;

        if (nextSymbol == null) 
            return;

        switch (_direction)
        {
            case Direction.North:
                _y -= nextSymbol.Height;
                break;
            case Direction.South:
                _y += symbol.Height;
                break;
            case Direction.East:
                _x += symbol.Width;
                break;
            case Direction.West:
                _x -= nextSymbol.Width;
                break;
        }
    }

    public override void OnResize(Bounds currentBounds)
    {
        if (Data.CurrentRace.Track.IsCentered)
            _trackSize = ConsoleSymbol.CalculateDimensionsOfTrack(Data.CurrentRace.Track);

        var top = currentBounds.Y + 2;
        _trackBounds = Bounds.CreateWithLeftTopRightBottom(2, top, currentBounds.Width - 2, currentBounds.Height);

        IsInvalidated = true;
    }

    public override void OnTrackInvalidate()
    {
        IsInvalidated = true;
    }

    public override void Dispose()
    {
        _consoleGraphicsRenderer.Dispose();
    }
}