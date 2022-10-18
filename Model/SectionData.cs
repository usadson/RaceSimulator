using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace Model
{
    public class SectionData
    {
        public class Lane
        {
            private IParticipant? _participant;

            public bool Changed = true;

            public uint Distance = 0;
            public IParticipant? Participant
            {
                get => _participant;
                set
                {
                    Changed = true;
                    _participant = value;
                    Distance = 0;
                }
            }
        }
        
        public bool Changed
        {
            get => Left.Changed || Right.Changed;
            set => Left.Changed = Right.Changed = value;
        }

        [NotNull] public Lane Left { get; } = new();
        [NotNull] public Lane Right { get; } = new();        
    }
}
