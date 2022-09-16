using System.Diagnostics.CodeAnalysis;

namespace Model
{
    public class Driver : IParticipant
    {
        public string Name { get; set; }
        public int Points { get; set; }
        public IEquipment Equipment { get; set; }
        public TeamColors TeamColor { get; set; }

        public Driver([DisallowNull] string name, int points, [DisallowNull] IEquipment equipment, 
            [DisallowNull] TeamColors teamColor)
        {
            Name = name;
            Points = points;
            Equipment = equipment;
            TeamColor = teamColor;
        }
    }
}
