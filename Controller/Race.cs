using Model;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Timer = System.Timers.Timer;

namespace Controller
{
    public sealed class Race
    {
        public const int Ticks = 10;

        public Track Track { get; set; }
        public List<IParticipant> Participants { get; private set;  }
        public DateTime StartTime { get; }

        private readonly Random _random = new(DateTime.Now.Millisecond);
        private Dictionary<Section, SectionData> _positions = new();

        public delegate void DriversChangedEventHandler(Race sender, DriversChangedEventArgs e);
        public event DriversChangedEventHandler? DriversChanged;

        public delegate void RaceEventHandler(Race sender);
        public event RaceEventHandler? ParticipantsOrderModified;
        public event RaceEventHandler? GameFinished;

        public delegate void ParticipantLappedEventHandler(Race sender, IParticipant participant, bool finished);
        public event ParticipantLappedEventHandler ParticipantLapped;

        private Timer _timer = new(100);
        private int _finishedParticipantsCount = 0;

        public Race([DisallowNull] Track track, [DisallowNull] List<IParticipant> participants)
        {
            Track = track;
            Participants = new(participants);
            StartTime = DateTime.Now;

            _timer.Elapsed += OnTimerElapsed;

            PlaceParticipants();
            MakeSureParticipantsAreSorted();
        }

        private void OnTimerElapsed(object? sender, EventArgs args)
        {
            lock (this)
            {
                MakeSureParticipantsAreSorted();

                if (_finishedParticipantsCount == Participants.Count)
                {
                    _timer.Stop();
                    return;
                }

                foreach (var participant in Participants)
                {
                    if (participant.Ranking != null)
                        continue;

                    var distanceToMove = participant.Equipment.Performance * participant.Equipment.Speed;
                    participant.DistanceFromStart += distanceToMove;
                    MoveParticipant(participant, (uint)distanceToMove);
                    VerifyPositions();
                }

                DriversChanged?.Invoke(this, new DriversChangedEventArgs(Track));
            }
        }

        private void MoveParticipant(IParticipant participant, uint amount)
        {
            Debug.Assert(participant.CurrentSection != null);

            var sectionData = GetSectionData(participant.CurrentSection);

            var isLeft = sectionData.Left.Participant == participant;
            
            var lane = isLeft ? sectionData.Left : sectionData.Right;
            Debug.Assert(lane.Participant == participant);
            
            lane.Distance += amount;

            var currentSectionIt = Track.Sections.Find(participant.CurrentSection);
            int overshot;

            uint sectionsParticipantWasForwaredTo = 0;
            while ((overshot = (int)lane.Distance - SectionRegistry.Lengths[participant.CurrentSection.SectionType]) > 0)
            {
                ++sectionsParticipantWasForwaredTo;

                Debug.Assert(currentSectionIt != null);

                var nextSectionIt = currentSectionIt.Next ?? Track.Sections.First;
                var nextSection = nextSectionIt?.Value;
                Debug.Assert(nextSection != null);

                if (nextSection.SectionType == SectionTypes.Finish)
                {
                    if (participant.Lap == Track.TotalLaps)
                    {
                        lane.Participant = null;

                        participant.Time = DateTime.Now - StartTime;
                        participant.Ranking = ++_finishedParticipantsCount;
                        participant.DistanceFromStart = int.MaxValue - (int)participant.Ranking;
                        ParticipantLapped?.Invoke(this, participant, true);

                        if (_finishedParticipantsCount == Participants.Count)
                            GameFinished?.Invoke(this);
                        return;
                    }

                    ++participant.Lap;

                    ParticipantLapped?.Invoke(this, participant, false);
                }

                var nextSectionData = GetSectionData(nextSection);

                var nextLane = isLeft ? nextSectionData.Left : nextSectionData.Right;

                if (nextLane.Participant != null)
                {
                    // participants cannot overtake other participants

                    if (isLeft && nextSectionData.Right.Participant == null)
                    {
                        isLeft = false;
                        nextLane = nextSectionData.Right;
                    }
                    else if (!isLeft && nextSectionData.Left.Participant == null)
                    {
                        isLeft = true;
                        nextLane = nextSectionData.Left;
                    }
                    else
                        break;
                }

                lane.Participant = null;
                lane = nextLane;

                lane.Participant = participant;
                lane.Distance = (uint)overshot;

                participant.CurrentSection = nextSection;
                currentSectionIt = nextSectionIt;
            }
        }

        public void Start()
        {
            _timer.Start();
        }

        [return: NotNull]
        public SectionData GetSectionData([DisallowNull] Section section)
        {
            lock (_positions)
            {
                if (_positions.TryGetValue(section, out SectionData? data))
                    return data;

                SectionData newData = new();
                _positions.Add(section, newData);
                return newData;
            }
        }

        public void RandomizeEquipment()
        {
            foreach (IParticipant participant in Participants)
            {
                participant.Equipment.Quality = _random.Next();
                participant.Equipment.Performance = _random.Next();
            }
        }

        private void MakeSureParticipantsAreSorted()
        {
            var sorted = new List<IParticipant>(Participants);
            sorted.Sort(new IParticipantDistanceComparer());
            if (!sorted.SequenceEqual(Participants))
            {
                Participants = sorted;
                ParticipantsOrderModified?.Invoke(this);
            }
        }

        private void PlaceParticipants()
        {
            Debug.Assert(_positions.Count == 0);
            var participants = new Queue<IParticipant>(Participants);

            foreach (var section in Track.Sections)
            {
                if (section.SectionType != SectionTypes.StartGrid)
                    continue;

                SectionData data = new();
                if (participants.TryDequeue(out var participant))
                {
                    data.Left.Participant = participant;
                    participant.CurrentSection = section;
                }
                else
                    return;

                if (participants.TryDequeue(out participant))
                {
                    data.Right.Participant = participant;
                    participant.CurrentSection = section;
                }
                
                _positions.Add(section, data);
            }
        }

        private int DistanceFromStart(Section section)
        {
            var sectionIt = Track.Sections.Find(section);
            Debug.Assert(sectionIt != null);

            LinkedListNode<Section> it = sectionIt.Previous ?? Track.Sections.Last;
            int distance = SectionRegistry.Lengths[section.SectionType];

            while (it.Value.SectionType != SectionTypes.Finish)
            {
                it = it.Previous ?? Track.Sections.Last;
                distance += SectionRegistry.Lengths[it.Value.SectionType];
            }

            return distance;
        }

        [Conditional("DEBUG")]
        private void VerifyPositions()
        {
            HashSet<IParticipant> encounteredParticipants = new();
            foreach (var position in _positions.Values)
            {
                if (position.Left.Participant != null
                        && !encounteredParticipants.Add(position.Left.Participant))
                {
                    Debug.Assert(false, $"Double participant: {position.Left.Participant}");
                }

                if (position.Right.Participant != null
                        && !encounteredParticipants.Add(position.Right.Participant))
                {
                    Debug.Assert(false, $"Double participant: {position.Right.Participant}");
                }
            }

            foreach (var participant in Participants)
            {
                if (participant.Ranking != null)
                    continue;

                Debug.Assert(encounteredParticipants.Contains(participant),
                    $"Participant not participating: {participant}");

            }
        }

        public void CleanUp()
        {
            DriversChanged = null;
            ParticipantsOrderModified = null;
            GameFinished = null;
        }
    }
}
