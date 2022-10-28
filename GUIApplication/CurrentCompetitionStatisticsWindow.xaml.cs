using System.Windows.Threading;

namespace GUIApplication;

public partial class CurrentCompetitionStatisticsWindow
{
    public CurrentCompetitionStatisticsWindow()
    {
        InitializeComponent();

        if (DataContext is ParticipantsStatisticsView view)
        {
            view.Dispatcher += action => Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
        }
    }
}