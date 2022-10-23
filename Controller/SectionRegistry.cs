using Model;
using System.Diagnostics.CodeAnalysis;

namespace Controller
{
    public static class SectionRegistry
    {
        public static Dictionary<SectionTypes, ushort> Lengths { get; } = new()
        {
            {SectionTypes.Straight, 200},
            {SectionTypes.StartGrid, 200},
            {SectionTypes.Finish, 200},
            {SectionTypes.LeftCorner, 300},
            {SectionTypes.RightCorner, 300},
        };
    }
}