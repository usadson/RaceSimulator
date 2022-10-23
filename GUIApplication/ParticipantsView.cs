using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Controller;
using Model;
using System.Windows;

namespace GUIApplication;

public class ParticipantsView : INotifyCollectionChanged, INotifyPropertyChanged
{
    public struct ParticipantEntry : INotifyPropertyChanged
    {
        

        public string Name { get; }
        public ushort PositionInRace { get; }
        public IParticipant Participant { get; }
        public Character Character { get; }
        public Int32Rect PictureRect { get; }
        public event PropertyChangedEventHandler? PropertyChanged = null;

        public ParticipantEntry(IParticipant participant)
        {
            Name = I18N.Translate(participant.Name);
            PositionInRace = participant.PositionInRace;
            Participant = participant;
            Character = ((Driver)participant).Character;

            var rect = SpriteOffsets.MinimapOffsets[Character];
            PictureRect = new(rect.X, rect.Y, rect.Width, rect.Height);

            PropertyChanged?.Invoke(this, new(nameof(PictureRect)));
        }
    }

    public ObservableCollection<ParticipantEntry> Participants { get; set; }= new();
    public string CompetitionName
    {
        get
        {
            if (!Data.HasRace())
                return "";

            return I18N.Translate(Data.CurrentRace.Track.Cup.ToString());
        }
    }

    public string TrackName
    {
        get
        {
            if (!Data.HasRace())
                return "";

            return I18N.Translate(Data.CurrentRace.Track.Name);
        }
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    private Action<Action>? _dispatcher;

    public ParticipantsView()
    {
        Data.NewRaceStarting += race =>
        {
            race.ParticipantsOrderModified += UpdateParticipants;

            UpdateParticipants(race);
            NotifyCompetition();
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

    public void SetDispatcher(Action<Action> action)
    {
        _dispatcher = action;

        if (Data.HasRace())
        {
            UpdateParticipants(Data.CurrentRace);
            NotifyCompetition();
        }
    }

    private void NotifyCompetition()
    {
        _dispatcher?.Invoke(() => PropertyChanged?.Invoke(this, new(nameof(TrackName))));
        _dispatcher?.Invoke(() => PropertyChanged?.Invoke(this, new(nameof(CompetitionName))));
    }
}