using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using Model;

namespace GUIApplication;

using DataContextDispatcher = Action<Action>;

public abstract class ParticipantsStatisticsView : INotifyCollectionChanged, INotifyPropertyChanged
{
    public ObservableCollection<ParticipantEntry> Participants { get; set; } = new();

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected DataContextDispatcher? _dispatcher;

    public DataContextDispatcher? Dispatcher
    {
        get => _dispatcher;
        set
        {
            _dispatcher = value;
            if (_dispatcher != null)
                OnDispatcherPublished();
        }
    }

    protected abstract void OnDispatcherPublished();

    protected void PropertyChangedManually(string name)
    {
        _dispatcher?.Invoke(() =>
        {
            PropertyChanged?.Invoke(this, new(name));
        });
    }

    protected void UpdateParticipants(IEnumerable<IParticipant> participants, bool competition)
    {
        if (_dispatcher == null)
        {
            Debug.WriteLine("UpdateParticipants: dispatcher is null! :(");
            return;
        }

        _dispatcher(() =>
        {
            Participants.Clear();
            foreach (var participant in participants)
                Participants.Add(new ParticipantEntry(
                     participant, competition
                ));

            PropertyChanged?.Invoke(this, new(nameof(Participants)));
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
        });
    }
}