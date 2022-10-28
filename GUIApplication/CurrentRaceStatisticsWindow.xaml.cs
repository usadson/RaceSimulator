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
using System.Windows.Shapes;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;

namespace GUIApplication;

/// <summary>
/// Interaction logic for CurrentRaceStatisticsWindow.xaml
/// </summary>
public partial class CurrentRaceStatisticsWindow
{
    public CurrentRaceStatisticsWindow()
    {
        InitializeComponent();

        if (DataContext is ParticipantsStatisticsView view)
        {
            view.Dispatcher += action => Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
        }
    }
}