using System.Diagnostics.CodeAnalysis;

namespace Model
{
    public class Driver : IParticipant
    {
        public string Name => Character.ToString();
        public int Points { get; set; }
        public IEquipment Equipment { get; set; }
        public TeamColors TeamColor { get; }

        public readonly Character Character;

        public Driver([DisallowNull] Character character, int points, [DisallowNull] IEquipment equipment, 
            [DisallowNull] TeamColors teamColor)
        {
            Character = character;
            Points = points;
            Equipment = equipment;
            TeamColor = teamColor;
        }
    }
}
