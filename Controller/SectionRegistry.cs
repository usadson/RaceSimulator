using Model;
using System.Diagnostics.CodeAnalysis;

namespace Controller
{
    public static class SectionRegistry
    {
        public static Dictionary<SectionTypes, ushort> Lengths { get; } = new()
        {
            {SectionTypes.Straight, 20},
            {SectionTypes.StartGrid, 20},
            {SectionTypes.Finish, 20},
            {SectionTypes.LeftCorner, 30},
            {SectionTypes.RightCorner, 30},
        };
    }
}