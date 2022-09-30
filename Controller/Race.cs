using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model;

namespace Controller
{
    public sealed class Race
    {
        [DisallowNull] public Track Track { get; set; }
        [DisallowNull] public List<IParticipant> Participants { get; }
        [DisallowNull] public DateTime StartTime { get; }

        private readonly Random _random = new(DateTime.Now.Millisecond);
        private Dictionary<Section, SectionData> _positions = new();

        public Race([DisallowNull] Track track, [DisallowNull] List<IParticipant> participants)
        {
            Track = track;
            Participants = participants;
            StartTime = DateTime.Now;

            PlaceParticipants();
        }

        [return: NotNull]
        public SectionData GetSectionData([DisallowNull] Section section)
        {
            if (_positions.TryGetValue(section, out SectionData? data))
                return data;

            SectionData newData = new();
            _positions.Add(section, newData);
            return newData;
        }

        public void RandomizeEquipment()
        {
            foreach (IParticipant participant in Participants)
            {
                participant.Equipment.Quality = _random.Next();
                participant.Equipment.Performance = _random.Next();
            }
        }

        private void PlaceParticipants()
        {
            var participants = new Queue<IParticipant>(Participants);

            foreach (var section in Track.Sections)
            {
                if (section.SectionType != SectionTypes.StartGrid)
                    continue;

                SectionData data = new();
                if (participants.TryDequeue(out var participant))
                    data.Left = participant;
                else
                    return;

                if (participants.TryDequeue(out participant))
                    data.Right = participant;

                _positions.Add(section, data);
            }
        }
    }
}
