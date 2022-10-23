using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Controller;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;

namespace GUIApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Renderer? _renderer;
        private readonly object _renderLock = new();

        private int _frameCount;
        private readonly DispatcherTimer _dispatcherTimer;
        private ParticipantStatisticsWindow? _participantStatisticsWindow;
        private RaceStatisticsWindow? _raceStatisticsWindow;

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

        private void OnRaceChanged()
        {
            Data.CurrentRace.Start();
            Data.CurrentRace.DriversChanged += (_, _) =>
            {
                Render();
            };

            Data.CurrentRace.GameFinished += _ => Task.Run(() => {
                if (!Data.HasNextRace)
                    return;

                lock (Renderer.RenderLock)
                {
                    _renderer = null;
                    Data.CurrentRace.Dispose();
                    SpriteManager.ClearCache();

                    Data.NextRace();
                    OnRaceChanged();
                }
            });
        }

        private void Render()
        {
            Task.Run(() =>
            {
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
            if (_renderer == null)
                _renderer = new(new System.Drawing.Size((int)RenderSize.Width, (int)RenderSize.Height));
            return _renderer;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _dispatcherTimer.Stop();
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Data.Speed = e.NewValue / 100;
        }

        private void MenuParticipants_OnClick(object sender, RoutedEventArgs e)
        {
            _participantStatisticsWindow ??= new();
            _participantStatisticsWindow.Show();
            _participantStatisticsWindow.Activate();
            _participantStatisticsWindow.Closed += (_, _) => _participantStatisticsWindow = null;
        }

        private void MenuRace_OnClick(object sender, RoutedEventArgs e)
        {
            _raceStatisticsWindow ??= new();
            _raceStatisticsWindow.Show();
            _raceStatisticsWindow.Activate();
            _raceStatisticsWindow.Closed += (_, _) => _participantStatisticsWindow = null;
        }

        private void MenuExit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
            Application.Current.Shutdown(0);
        }
    }
}
