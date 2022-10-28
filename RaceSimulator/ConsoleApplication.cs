using Controller;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text;
using System.Timers;

namespace RaceSimulator;

[SupportedOSPlatform("windows")]
public sealed class ConsoleApplication : Application
{
    private (int Width, int Height) _currentSize = new(Console.WindowWidth, Console.WindowHeight);
        
    private bool _darkMode = ShouldSystemUseDarkMode();

    private DateTime? _timeSinceResize;

    private readonly ActionBar _actionBar = new();
    private readonly StatisticsHeader _header = new();
    private Screen? _screen;
    private readonly object _screenLock = new();

    private readonly DateTime _initialized;

    public ConsoleApplication()
    {
        //ConsoleAPI.Hook();

        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = I18N.Translate("WindowTitle");

        Console.CursorVisible = false;
        Console.SetWindowSize(100, 30);

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
    }
        
    public override void Run()
    {
        _actionBar.Show(I18N.Translate("GameStarted"), 3000);

        OnNewTrackBegin();
        OnThemeChanged();

        _screen = new TrackScreen();
        OnResize(new(Console.WindowWidth, Console.WindowHeight));
        OnTrackChanged();

        Console.Clear();
        _header.IsInvalidated = true;

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
                    
                Draw();
            }
        }
    }

    private void ShowResultsScreen(Task task)
    {
        Console.Clear();
        _header.IsInvalidated = true;

        foreach (var participant in Data.CurrentRace.Participants)
        {
            Debug.Assert(participant.Time != null, "participant.Time != null");
            participant.CompetitionPoints += participant.Time.Value.Milliseconds;
        }

        lock (_screenLock)
        {
            if (Data.HasNextRace)
                _screen = new ResultsScreen(Data.CurrentRace.Participants);
            else
            {
                _screen = new CompetitionFinishedScreen();
            }
        }

        foreach (var participant in Data.CurrentRace.Participants)
        {
            participant.CurrentSection = null;
            participant.DistanceFromStart = 0;
            participant.Lap = 0;
            participant.Ranking = null;
            participant.Time = null;
        }

        Data.CurrentRace.Dispose();
        if (!Data.HasNextRace)
            return;

        Data.NextRace();

        var timer = new System.Timers.Timer(1000);
        int countdown = 5;

        void ElapsedEventHandler(object? o, ElapsedEventArgs? elapsedEventArgs)
        {
            if (countdown == -1)
            {
                StartNewGame();
                timer.Stop();
                return;
            }

            var messageKey = $"NewGame{countdown}s";
            var message = I18N.Translate(messageKey);
            if (message == messageKey) message = I18N.Translate("NewGameNs").Replace("SECONDS", countdown.ToString());

            // Milliseconds more than 1000, even though it will be reset by
            // the next invocation of this timer, because a hang might occur.
            _actionBar.Show(message, 1100);

            --countdown;
        }

        ElapsedEventHandler(null, null);
        timer.Elapsed += ElapsedEventHandler;
        timer.Start();
    }

    private void StartNewGame()
    {
        lock (_screenLock)
        {
            _screen?.Dispose();
            _screen = new TrackScreen();
            _screen?.OnResize(new(0, _header.Height, Console.WindowWidth, Console.WindowHeight));
        }

        Console.Clear();
        _header.IsInvalidated = true;
        OnTrackInvalidated();
        OnNewTrackBegin();
    }

    private void OnNewTrackBegin()
    {
        Data.CurrentRace.Start();

        Data.CurrentRace.DriversChanged += (_, _) => OnTrackInvalidated();
        Data.CurrentRace.ParticipantsOrderModified += _ => _header.IsInvalidated = true;
            
        Data.CurrentRace.ParticipantLapped += (_, participant, didFinish) =>
        {
            _header.IsInvalidated = true;

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

        Data.CurrentRace.GameFinished += _ =>
        {
            const ushort ms = 3000;
            _actionBar.Show(I18N.Translate("GameFinished"), ms);
            Task.Delay(ms).ContinueWith(ShowResultsScreen);
        };
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

        if (_header.IsInvalidated && IsTrackInvalidated())
            return;

        _header.IsInvalidated = true;
        OnTrackInvalidated();

        Console.Clear();
    }

    private void Draw()
    {
        if (_header.IsInvalidated)
            _header.Draw(_darkMode);

        lock (_screenLock)
        lock (Data.CurrentRace)
            _screen?.Draw(_header.Height);

        _actionBar.Draw();
    }

    private void OnTrackChanged()
    {
        _header.IsInvalidated = true;
        OnTrackInvalidated();
    }

    private void OnResize((int Width, int Height) size)
    {
        _timeSinceResize = DateTime.Now;
        _currentSize = size;
        Console.SetBufferSize(Console.WindowLeft + Console.WindowWidth, Console.WindowTop + Console.WindowHeight);
        _header.IsInvalidated = true;
        OnTrackInvalidated();
        Data.CurrentRace.NotifyAllChanged();

        _screen?.OnResize(new(0, _header.Height, Console.WindowWidth, Console.WindowHeight));
    }

    public override void Update()
    {
        (int Width, int Height) size = new(Console.WindowWidth, Console.WindowHeight);
        if (size.Width != _currentSize.Width || size.Height != _currentSize.Height)
            OnResize(size);

        Console.CursorVisible = false;

        base.Update();
    }

    private bool IsTrackInvalidated()
    {
        if (_screen is TrackScreen trackScreen)
            return trackScreen.IsInvalidated;
        return false;
    }

    private void OnTrackInvalidated()
    {
        lock (_screenLock)
            _screen?.OnTrackInvalidate();
    }
}