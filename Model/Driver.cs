using System.Diagnostics.CodeAnalysis;

namespace Model
{
    public sealed class Driver : IParticipant
    {
        public string Name => Character.ToString();
        public int Points { get; set; }
        public IEquipment Equipment { get; set; }
        public TeamColors TeamColor { get; }

        public Section? CurrentSection { get; set; }
        public int DistanceFromStart { get; set; }
        public int Lap { get; set; } = 0;
        public int? Ranking { get; set; } = null;
        public string LapStringified { 
            get {
                if (Lap == 0)
                    return "1";

                return Lap.ToString();
            }
        }
        public TimeSpan? Time { get; set; }

        public readonly Character Character;

        public Driver(Character character, int points, IEquipment equipment, TeamColors teamColor)
        {
            Character = character;
            Points = points;
            Equipment = equipment;
            TeamColor = teamColor;
        }

        public override string ToString()
        {
            return $"Driver name({Name}) DFS({DistanceFromStart})";
        }
    }
}
