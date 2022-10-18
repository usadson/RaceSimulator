using Controller;
using Model;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace RaceSimulator
{
    public sealed class ConsoleApplication : Application
    {
        private bool _headerInvalidated = true;
        private bool _trackInvalidated = true;
        private Direction _direction = Direction.West;
        private int X = 0, Y = 0;

        private (int Width, int Height) _currentSize = new(Console.WindowWidth, Console.WindowHeight);
        private Bounds _trackBounds = new(0, 0, 0, 0);
        private Bounds _trackSize = new(0, 0, 0, 0);
        private bool _darkMode = ShouldSystemUseDarkMode();
        private int _headerHeight = 0;

        private DateTime? _timeSinceResize;

        private readonly ActionBar _actionBar = new();
        private Screen? _screen;

        private readonly DateTime _initialized;

        [DllImport("kernel32.dll", EntryPoint = "GetConsoleWindow", SetLastError = true)]
        private static extern IntPtr GetConsoleHandle();

        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr windowHandle, out Rectangle rect);

        [DllImport("user32.dll")]
        private static extern int UpdateWindow(IntPtr windowHandle);

        private readonly Image _bitmap = Image.FromFile("C:\\Data\\dogs.jpg");
        private IntPtr _handle;

        private readonly Rectangle _rect;

        public ConsoleApplication()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = I18N.Translate("WindowTitle");

            Console.CursorVisible = false;
            Console.WindowWidth = 100;
            Console.WindowHeight = 30;

            OnThemeChanged();

            var splashBegin = DateTime.Now;
            Debug.Assert(Console.WindowWidth > 0);
            Debug.Assert(Console.WindowHeight > 0);
            Console.Clear();
            string text = I18N.Translate("SplashText");
            Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, Console.WindowHeight / 2);
            Console.Write(text);
            Debug.WriteLine($"Splash screen took {(DateTime.Now - splashBegin).TotalMilliseconds} ms");

            _initialized = DateTime.Now;

#if not
            _handle = GetConsoleHandle();
            GetWindowRect(_handle, out _rect);
#endif
        }
        
        public override void Run()
        {
            _actionBar.Show(I18N.Translate("GameStarted"), 3000);

            Data.CurrentRace.Start();
            Data.CurrentRace.DriversChanged += (_, _) =>
            {
                _trackInvalidated = true;
            };
            Data.CurrentRace.ParticipantsOrderModified += (_) =>
            {
                _headerInvalidated = true;
            };
            Data.CurrentRace.ParticipantLapped += (_, participant, didFinish) =>
            {
                _headerInvalidated = true;

                if (!didFinish) 
                    return;

                var translationKey = "FinishedN";
                Debug.Assert(participant.Ranking != null);
                if (participant.Ranking <= 3)
                    translationKey = "Finished" + participant.Ranking;

                _actionBar.Show(I18N.Translate(translationKey)
                        .Replace("NAME", I18N.Translate(participant.Name))
                        .Replace("POSITION", ((int)participant.Ranking).ToString()),
                    1500
                );
            };
            Data.CurrentRace.GameFinished += (_) =>
            {
                const ushort ms = 3000;
                _actionBar.Show(I18N.Translate("GameFinished"), ms);
                Task.Delay(ms).ContinueWith(ShowResultsScreen);
            };


            OnThemeChanged();
            OnResize(new(Console.WindowWidth, Console.WindowHeight));
            OnTrackChanged();

            Console.Clear();
            _headerInvalidated = true;

            Debug.WriteLine("Initialized -> gameStart took {0} ms", (DateTime.Now - _initialized).TotalMilliseconds);

            while (!Console.KeyAvailable)
            {
                lock (Data.CurrentRace)
                {
                    Update();

                    if (_timeSinceResize != null)
                    {
                        if ((DateTime.Now - _timeSinceResize).Value.TotalMilliseconds > 100)
                        {
                            Console.Clear();
                            _timeSinceResize = null;
                        }
                        else
                            continue;
                    }

                    var mode = ShouldSystemUseDarkMode();
                    if (mode != _darkMode)
                    {
                        _darkMode = mode;
                        OnThemeChanged();
                    }
                    
                    lock (Data.CurrentRace)
                    {
                        Draw();
                    }
                }
            }
        }

        private void ShowResultsScreen(Task task)
        {
            Console.Clear();
            _headerInvalidated = true;

            _screen = new ResultsScreen(Data.CurrentRace.Participants);

            Data.CurrentRace.CleanUp();
            Data.NextRace();
            
            Task.Delay(5000).ContinueWith(_ =>
            {
                _screen = null;

                Console.Clear();
                _headerInvalidated = true;
                _trackInvalidated = true;
            });
        }

        private void OnThemeChanged()
        {
            if (_darkMode)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            if (_headerInvalidated && _trackInvalidated)
                return;

            _headerInvalidated = _trackInvalidated = true;

            Console.Clear();
        }

        public void Draw()
        {
            if (_headerInvalidated)
                DrawHeader();

            if (_screen != null)
                _screen.Draw(_trackBounds.Top);
            else if (_trackInvalidated)
                DrawTrack();

            _actionBar.Draw();

            DrawIcon();
        }

        private void OnTrackChanged()
        {
            _headerInvalidated = true;
            _trackInvalidated = true;

            if (Data.CurrentRace.Track.IsCentered)
                _trackSize = CalculateDimensionsOfTrack(Data.CurrentRace.Track);
        }

        private void OnResize((int Width, int Height) size)
        {
            _timeSinceResize = DateTime.Now;
            _currentSize = size;
            Console.SetBufferSize(Console.WindowLeft + Console.WindowWidth, Console.WindowTop + Console.WindowHeight);
            _headerInvalidated = true;
            _trackInvalidated = true;
            Data.CurrentRace.NotifyAllChanged();

            DrawHeader();

            var top = _headerHeight + 2;
            _trackBounds = Bounds.CreateWithLeftTopRightBottom(2, top, _currentSize.Width - 2, _currentSize.Height);
        }

        public override void Update()
        {
            (int Width, int Height) size = new(Console.WindowWidth, Console.WindowHeight);
            if (size.Width != _currentSize.Width || size.Height != _currentSize.Height)
                OnResize(size);

            Console.CursorVisible = false;

            base.Update();
        }

        private void DrawHeader()
        {
            _headerInvalidated = false;
            Console.SetCursorPosition(Console.WindowLeft, Console.WindowTop);
            Console.WriteLine();

            Console.ForegroundColor = _darkMode ? ConsoleColor.Yellow : ConsoleColor.DarkYellow;
            Console.Write("  Pista di Gara: ");
            Console.ForegroundColor = _darkMode ? ConsoleColor.Gray : ConsoleColor.Black;
            Console.Write($"{I18N.Translate(Data.CurrentRace.Track.Name)}");
            FillLineTillEnd();

            Console.ForegroundColor = _darkMode ? ConsoleColor.Yellow : ConsoleColor.DarkYellow;
            Console.Write("  Giocatori: ");
            Console.ForegroundColor = _darkMode ? ConsoleColor.Gray : ConsoleColor.Black;
            Console.Write(
                string.Join(", ",
                    Data.CurrentRace.Participants.Select(
                        participant => string.Format($"{I18N.Translate(((Driver)participant).Character.ToString())} {participant.LapStringified}")
            )));
            FillLineTillEnd();
            FillLineTillEnd();
            FillLineTillEnd('=');

            _headerHeight = Console.GetCursorPosition().Top;
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
                Debug.Assert(_trackSize.MiddleX != 0);
                Debug.Assert(_trackSize.MiddleY != 0);
                X = _trackBounds.MiddleX - _trackSize.MiddleX;
                Y = _trackBounds.MiddleY - _trackSize.MiddleY;
                if (Y + _trackSize.Height > Console.WindowHeight)
                {
                    if (_trackSize.Height + _headerHeight > Console.WindowHeight)
                        Y = Console.WindowHeight - _trackSize.Height;
                    else 
                        Y = _headerHeight - _trackSize.MiddleY + _trackSize.Height;
                }
            }
            else
            {
                X = _trackBounds.X + Data.CurrentRace.Track.XOffset;
                Y = _trackBounds.Y + Data.CurrentRace.Track.YOffset;
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
                if (data.Left.Participant != null && data.Left.Participant.Ranking == null)
                    repl1 = I18N.Translate("Letter of " + ((Driver)data.Left.Participant).Character);

                if (data.Right.Participant != null && data.Right.Participant.Ranking == null)
                    repl2 = I18N.Translate("Letter of " + ((Driver)data.Right.Participant).Character);

                if (data.Changed)
                {
                    symbol.Draw(X, Y, _trackBounds, repl1, repl2);
                    data.Changed = false;
                }

                switch (section.SectionType)
                {
                    case SectionTypes.LeftCorner:
                        _direction = RotateLeft(_direction);
                        break;
                    case SectionTypes.RightCorner:
                        _direction = RotateRight(_direction);
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

            int minX = 0;
            int minY = 0;

            var direction = track.BeginDirection;

            var it = track.Sections.First;
            while (it != null)
            {
                var section = it.Value;
                it = it.Next;
                var nextSection = it?.Value;

                var symbol = FindSymbol(section.SectionType, direction);
                    
                Debug.Assert(symbol != null);

                var outerX = x + symbol.Width;
                var outerY = y + symbol.Height;
                Debug.WriteLine($"x={x} outerX={outerX}     y={y} outerY={outerY} minY={minY}");
                
                if (maxX < outerX)
                    maxX = outerX;
                if (maxY < outerY)
                    maxY = outerY;

                if (minX > x)
                    minX = x;
                if (minY > y)
                    minY = y;

                switch (section.SectionType)
                {
                    case SectionTypes.LeftCorner:
                        direction = RotateLeft(direction);
                        break;
                    case SectionTypes.RightCorner:
                        direction = RotateRight(direction);
                        break;
                }

                if (nextSection == null)
                    continue;

                var nextSymbol = FindSymbol(nextSection.SectionType, direction);
                Debug.Assert(nextSymbol != null);

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

            Debug.WriteLine($"minX={minX} maxX={maxX}");
            Debug.WriteLine($"minY={minY} maxY={maxY}");
            return new Bounds(minX, minY, maxX, maxY);
        }

        private void DrawIcon()
        {
            int width = _rect.Width;
            int height = _rect.Height;

            if (width + height != 0 || _handle.Equals(IntPtr.Zero))
                return;

            // using (var bmp = new Bitmap(600, 400))
            // using (var gfx = Graphics.FromImage(bmp))
            using (var gfx = Graphics.FromHwnd(_handle))
            {
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                Random rand = new Random(0);
                Pen pen = new Pen(Color.White);

                gfx.DrawImage(_bitmap, new Point(1, 1));
                // bmp.Save(@"C:\temp\demo.png");
            }

            UpdateWindow(_handle);
        }

    }
}
