using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Controller;
using Model;
using System.Windows;
using System.Windows.Threading;

namespace GUIApplication;

using DataContextDispatcher = Action<Action>;

public class ParticipantsStatisticsView : INotifyCollectionChanged, INotifyPropertyChanged
{
    public struct ParticipantEntry : INotifyPropertyChanged
    {
        public static readonly Int32Rect[] PositionRects = {
            new(31, 1, 108, 119), // 1
            new(169, 3, 112, 116), // 2
            new(319, 3, 120, 112), // 3
            new(479, 4, 122, 110), // 4
            new(31, 139, 131, 112), // 5
            new(187, 138, 121, 114), // 6
            new(325, 136, 122, 115), // 7
            new(479, 133, 128, 116), // 8
            new(1, 271, 118, 116), // 9
            new(140, 266, 168, 121), // 10
            new(313, 268, 166, 118), // 11
            new(497, 267, 166, 119) // 12
        };

        public string Name { get; }
        public ushort PositionInRace { get; }
        public IParticipant Participant { get; }
        public Character Character { get; }
        public Int32Rect PictureRect { get; }
        public Int32Rect PositionRect { get; }
        public event PropertyChangedEventHandler? PropertyChanged = null;

        public ParticipantEntry(IParticipant participant)
        {
            Name = I18N.Translate(participant.Name);
            PositionInRace = participant.PositionInRace;
            Participant = participant;
            Character = ((Driver)participant).Character;

            var rect = SpriteOffsets.MinimapOffsets[Character];
            PictureRect = new(rect.X, rect.Y, rect.Width, rect.Height);

            var index = PositionInRace - 1;
            if (index >= 0 && index < PositionRects.Length)
                PositionRect = PositionRects[PositionInRace - 1];
            else
                PositionRect = PositionRects[^1];

            PropertyChanged?.Invoke(this, new(nameof(PictureRect)));
        }
    }

    public ObservableCollection<ParticipantEntry> Participants { get; set; }= new();

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    private DataContextDispatcher? _dispatcher;

    public DataContextDispatcher? Dispatcher
    {
        get => _dispatcher;
        set
        {
            _dispatcher = value;
            if (_dispatcher != null && Data.HasRace())
            {
                UpdateParticipants(Data.CurrentRace);
            }
        }
    }

    public ParticipantsStatisticsView()
    {
        Data.NewRaceStarting += race =>
        {
            race.ParticipantsOrderModified += UpdateParticipants;

            UpdateParticipants(race);
        };
    }
    
    private void UpdateParticipants(Race race)
    {
        if (_dispatcher == null)
        {
            Debug.WriteLine("UpdateParticipants: dispatcher is null! :(");
            return;
        }
        
        _dispatcher(() =>
        {
            Participants.Clear();
            foreach (var participant in race.Participants)
                Participants.Add(new ParticipantEntry(
                    participant    
                ));

            PropertyChanged?.Invoke(this, new(nameof(Participants)));
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
        });
    }
}