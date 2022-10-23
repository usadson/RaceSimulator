using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Controller;

namespace GUIApplication;

public sealed class RaceSimulatorDataContext : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public RaceSimulatorDataContext()
    {
        Data.NewRaceStarting += race =>
        {
            race.DriversChanged += (sender, _) =>
            {
                PropertyChanged?.Invoke(sender, new(""));
            };
        };
    }

    public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) 
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
