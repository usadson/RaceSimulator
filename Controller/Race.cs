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
    }
}
