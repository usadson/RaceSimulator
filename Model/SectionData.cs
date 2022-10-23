using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace Model
{
    public class SectionData : ICloneable
    {
        public class Lane
        {
            private IParticipant? _participant;

            public bool Changed = true;

            public uint Distance;
            public IParticipant? Participant
            {
                get => _participant;
                set
                {
                    if (value is not null)
                        Debug.Assert(_participant == null);

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

        public Lane Left { get; } = new();
        public Lane Right { get; } = new();

        public object Clone()
        {
            return new SectionData
            {
                Left =
                {
                    Participant = Left.Participant,
                    Distance = Left.Distance
                },
                Right =
                {
                    Participant = Right.Participant,
                    Distance = Right.Distance
                }
            };
        }
    }
}
