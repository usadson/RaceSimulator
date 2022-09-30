using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RaceSimulator
{
    public sealed class ConsoleApplication : Application
    {
        private bool _headerInvalidated = true;
        private bool _trackInvalidated = true;
        private Direction _direction = Direction.West;
        private int X = 0, Y = 0;

        private (int Width, int Height) CurrentSize = new(Console.WindowWidth, Console.WindowHeight);
        private Bounds TrackBounds = new(0, 0, 0, 0);
        private Bounds TrackSize = new(0, 0, 0, 0);

        public override void Run()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Carro di Mario";
            Console.CursorVisible = false;
            OnResize(new(Console.WindowWidth, Console.WindowHeight));
            OnTrackChange();
            Console.Clear();
            DrawHeader();
            SetupEvents();

            while (true)
            {
                Update();
                Draw();
            }
        }

        public void Draw()
        {
            if (_headerInvalidated)
                DrawHeader();

            if (_trackInvalidated)
                DrawTrack();
        }

        private void OnTrackChange()
        {
            _headerInvalidated = true;
            _trackInvalidated = true;

            if (Data.CurrentRace.Track.IsCentered)
                TrackSize = CalculateDimensionsOfTrack(Data.CurrentRace.Track);
        }

        private void OnResize((int Width, int Height) size)
        {
            CurrentSize = size;
            //Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
            _headerInvalidated = true;
            _trackInvalidated = true;

            TrackBounds = Bounds.CreateWithLeftTopRightBottom(2, 6, CurrentSize.Width - 2, CurrentSize.Height);
        }

        public override void Update()
        {
            base.Update();

            (int Width, int Height) size = new(Console.WindowWidth, Console.WindowHeight);
            if (size.Width != CurrentSize.Width || size.Height != CurrentSize.Height)
                OnResize(size);
        }

        private void DrawHeader()
        {
            _headerInvalidated = false;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("  Pista di Gara: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{I18N.Translate(Data.CurrentRace.Track.Name)}");
            FillLineTillEnd();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("  Giocatori: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(
                string.Join(", ",
                    Data.CurrentRace.Participants.Select(
                        participant => I18N.Translate(((Driver)participant).Character.ToString())
            )));
            FillLineTillEnd();
            FillLineTillEnd();
            FillLineTillEnd('=');
        }

        private void DrawTrack()
        {
            _trackInvalidated = false;
#if FALSE
            int lastY = Console.WindowHeight - 1;
            for (int y = Console.CursorTop; y < lastY; ++y)
            {
                bool lastLine = y == lastY - 1;
                Console.Write("  ");

                FillLineTillEnd('.', !lastLine, 2);
            }
#endif

            _direction = Data.CurrentRace.Track.BeginDirection;
            
            if (Data.CurrentRace.Track.IsCentered)
            {
                X = TrackBounds.MiddleX - TrackSize.MiddleX;
                Y = TrackBounds.MiddleY - TrackSize.MiddleY;
            }
            else
            {
                X = TrackBounds.X + Data.CurrentRace.Track.XOffset;
                Y = TrackBounds.Y + Data.CurrentRace.Track.YOffset;
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
            var symbol = FindSymbol(section.SectionType, _direction);

            if (symbol != null)
            {
                string repl1 = " ";
                string repl2 = " ";
                if (data.Left != null)
                    repl1 = I18N.Translate("Letter of " + ((Driver)data.Left).Character);

                if (data.Right != null)
                    repl2 = I18N.Translate("Letter of " + ((Driver)data.Right).Character);

                symbol.Draw(X, Y, TrackBounds, repl1, repl2);

                switch (section.SectionType)
                {
                    case SectionTypes.LeftCorner:
                        _direction = RotateLeft(_direction);
                        break;
                    case SectionTypes.RightCorner:
                        _direction = RotateRight(_direction);
                        break;
                    default:
                        break;
                }

                var nextSymbol = nextSection != null ? FindSymbol(nextSection.SectionType, _direction) : null;

                if (nextSymbol != null)
                {
                    switch (_direction)
                    {
                        case Direction.North:
                            Y -= nextSymbol.Height;
                            break;
                        case Direction.South:
                            Y += symbol.Height;
                            break;
                        case Direction.East:
                            X += symbol.Width;
                            break;
                        case Direction.West:
                            X -= nextSymbol.Width;
                            break;
                    }
                }
            }
        }

        private static void FillLineTillEnd(char character = ' ', bool line = true, int paddingRight = 0)
        {
            Console.Write(new string(character, Console.WindowWidth - Console.CursorLeft - paddingRight));
            if (line)
                Console.WriteLine();
        }

        private void SetupEvents()
        {
            //ConsoleWindowManager.Initialize();
        }

        private static Direction RotateLeft(Direction direction)
        {
            return direction switch
            {
                Direction.North => Direction.West,
                Direction.East => Direction.North,
                Direction.South => Direction.East,
                Direction.West => Direction.South,
                _ => throw new InvalidEnumArgumentException(),
            };
        }

        private static Direction RotateRight(Direction direction)
        {
            return direction switch
            {
                Direction.North => Direction.East,
                Direction.East => Direction.South,
                Direction.South => Direction.West,
                Direction.West => Direction.North,
                _ => throw new InvalidEnumArgumentException(),
            };
        }

        private static ConsoleSymbol? FindSymbol(SectionTypes sectionType, Direction direction)
        {
            if (ConsoleSymbol.Symbols.TryGetValue(sectionType, out var dict))
            {
                if (dict.TryGetValue(direction, out var symbol))
                    return symbol;
            }

            return null;
        }

        private static Bounds CalculateDimensionsOfTrack(Track track)
        {
            int x = 0;
            int y = 0;

            int maxX = x;
            int maxY = y;

            var direction = Data.CurrentRace.Track.BeginDirection;

            var it = track.Sections.First;
            while (it != null)
            {
                var section = it.Value;
                it = it.Next;
                var nextSection = it?.Value;

                var symbol = FindSymbol(section.SectionType, direction);

                if (symbol == null)
                    break;

                var outerX = x + symbol.Width;
                var outerY = y + symbol.Height;
                
                if (maxX < outerX)
                    maxX = outerX;
                if (maxY < outerY)
                    maxY = outerY;

                switch (section.SectionType)
                {
                    case SectionTypes.LeftCorner:
                        direction = RotateLeft(direction);
                        break;
                    case SectionTypes.RightCorner:
                        direction = RotateRight(direction);
                        break;
                    default:
                        break;
                }

                var nextSymbol = nextSection != null ? FindSymbol(nextSection.SectionType, direction) : null;

                if (nextSymbol == null)
                    break;

                switch (direction)
                {
                    case Direction.North:
                        y -= nextSymbol.Height;
                        break;
                    case Direction.South:
                        y += symbol.Height;
                        break;
                    case Direction.East:
                        x += symbol.Width;
                        break;
                    case Direction.West:
                        x -= nextSymbol.Width;
                        break;
                }
            }

            return new(0, 0, maxX, maxY);
        }

    }
}
