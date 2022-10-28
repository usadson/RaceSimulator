using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Controller;
using Model;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;

namespace GUIApplication;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private Renderer? _renderer;
    private readonly object _renderLock = new();

    private int _frameCount;
    private readonly DispatcherTimer _dispatcherTimer;
    private CurrentRaceStatisticsWindow? _currentRaceStatisticsWindow;
    private CurrentCompetitionStatisticsWindow? _currentCompetitionStatisticsWindow;

    public MainWindow()
    {
        InitializeComponent();
        I18N.Initialize();

        _dispatcherTimer = new();
        _dispatcherTimer.Tick += DispatcherTimerOnTick;
        _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        _dispatcherTimer.Start();

        ((ParticipantsView)DataContext).SetDispatcher(action =>
        {
            Dispatcher.BeginInvoke(action, DispatcherPriority.Normal);
        });
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        Task.Run(() =>
        {
            Data.Initialize();
            SortCompetitionParticipants();
            Data.NextRace();
            OnRaceChanged();
        });
    }

    private void DispatcherTimerOnTick(object? sender, EventArgs e)
    {
        var frameCount = _frameCount;
        _frameCount = 0;

        Title = $"{I18N.Translate("WindowTitle")} (FPS: {frameCount})";
    }

    private static int CalculateCompetitionPoints(int? ranking, int participantCount)
    {
        Debug.Assert(ranking != null);

        if (participantCount > 12)
        {
            return (int)ranking;
        }

        return ranking switch
        {
            1 => 15,
            2 => 12,
            3 => 10,
            4 => 8,
            5 => 7,
            6 => 6,
            7 => 5,
            8 => 4,
            9 => 3,
            10 => 2,
            11 => 1,
            12 => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(ranking), ranking, "Must be => 1 && <= 12")
        };
    }

    private void OnRaceChanged()
    {
        Data.CurrentRace.Start();
        Data.CurrentRace.DriversChanged += (_, _) =>
        {
            Render();
        };

        Data.CurrentRace.GameFinished += _ => Task.Run(() => {
            lock (Renderer.RenderLock)
            {
                UpdateCompetitionAfterRace();

                _renderer = null;
                Data.CurrentRace.Dispose();
                SpriteManager.ClearCache();

                Data.NextRace();

                if (Data.HasRace())
                    OnRaceChanged();
                else
                    OnCompetitionEnded();
            }
        });
    }

    private void OnCompetitionEnded()
    {
        Dispatcher.BeginInvoke(() =>
        {
            VictoryImage.Visibility = Visibility.Visible;
        });
    }

    private void UpdateCompetitionAfterRace()
    {
        foreach (var participant in Data.CurrentRace.Participants)
        {
            participant.CompetitionPoints += CalculateCompetitionPoints(participant.Ranking,
                Data.CurrentRace.Participants.Count);
        }

        SortCompetitionParticipants();
    }

    private void SortCompetitionParticipants() => Data.CurrentCompetition?.Participants.Sort(new IParticipantCompetitionComparer());

    private void Render()
    {
        Task.Run(() =>
        {
            if (!Data.HasRace())
                return;
            if (RenderSize.Width == 0 || RenderSize.Height == 0)
                return;

            lock (_renderLock)
            {
                var renderer = GetRenderer();
                renderer.FrameSize = new((int)RenderSize.Width, (int)RenderSize.Height);
                var bitmapSource = renderer.Draw();
                bitmapSource.Freeze();
                GameFrame.Dispatcher.BeginInvoke(DispatcherPriority.Render, () =>
                {
                    GameFrame.Source = null;
                    GameFrame.Source = bitmapSource;
                });

                ++_frameCount;
            }
        });
    }

    private Renderer GetRenderer()
    {
        _renderer ??= new(new System.Drawing.Size((int)RenderSize.Width, (int)RenderSize.Height));
        return _renderer;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        OnClosed();
    }

    // Separated from OnClosed(EventArgs) to help mocking.
    public void OnClosed()
    {
        _currentCompetitionStatisticsWindow?.Close();
        _currentRaceStatisticsWindow?.Close();

        if (Data.HasRace())
            Data.CurrentRace.Dispose();

        _dispatcherTimer.Stop();
    }

    private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        Data.Speed = e.NewValue / 100;
    }

    private void MenuCompetition_OnClick(object sender, RoutedEventArgs e)
    {
        if (_currentCompetitionStatisticsWindow is not { Visibility: Visibility.Visible })
            _currentCompetitionStatisticsWindow = new();

        _currentCompetitionStatisticsWindow.Show();
        _currentCompetitionStatisticsWindow.Activate();
        _currentCompetitionStatisticsWindow.Closing += (_, _) => _currentCompetitionStatisticsWindow = null;
    }

    private void MenuRace_OnClick(object sender, RoutedEventArgs e)
    {
        _currentRaceStatisticsWindow ??= new();
        _currentRaceStatisticsWindow.Show();
        _currentRaceStatisticsWindow.Activate();
        _currentRaceStatisticsWindow.Closed += (_, _) => _currentRaceStatisticsWindow = null;
    }

    private void MenuExit_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
        Application.Current.Shutdown(0);
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        lock (_renderLock)
        {
            _renderer = null;
            SpriteManager.ClearCache();
        }
    }

    private void MenuLanguage_English_OnClick(object sender, RoutedEventArgs e)
    {
        App.SetLanguage("en-US");
    }

    private void MenuLanguage_Italian_OnClick(object sender, RoutedEventArgs e)
    {
        App.SetLanguage("it-IT");
    }
}
