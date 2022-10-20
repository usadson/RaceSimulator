using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private object _renderLock = new();

        public MainWindow()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                Data.Initialize();
                Data.NextRace();
                OnRaceChanged();
            });
        }

        private void OnRaceChanged()
        {
            Data.CurrentRace.Start();
            Data.CurrentRace.DriversChanged += (_, _) => Render();
        }

        private void Render()
        {
            Task.Run(() =>
            {
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
                }
            });
        }

        private Renderer GetRenderer()
        {
            if (_renderer == null)
                _renderer = new(Data.CurrentRace, new System.Drawing.Size((int)RenderSize.Width, (int)RenderSize.Height));
            return _renderer;
        }
    }
}
