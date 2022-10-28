namespace Model;

public sealed class Driver : IParticipant
{
    public string Name => Character.ToString();
    public int Points { get; set; }
    public IEquipment Equipment { get; set; }
    public TeamColors TeamColor { get; }

    public Section? CurrentSection { get; set; }
    public int DistanceFromStart { get; set; }
    public int Lap { get; set; }
    public int? Ranking { get; set; }
    public ushort PositionInRace { get; set; }

    public ushort PositionInCompetition
    {
        get
        {
            if (_competition == null)
                return 12;
            return (ushort)(_competition.Participants.IndexOf(this) + 1);
        }
    }

    private Competition? _competition;

    public void OnNewRace(Competition competition)
    {
        DistanceFromStart = 0;
        Lap = 0;
        Ranking = null;
        PositionInRace = 0;

        _competition = competition;
    }

    public string LapStringified { 
        get {
            if (Lap == 0)
                return "1";

            return Lap.ToString();
        }
    }
    public TimeSpan? Time { get; set; }
    public int CompetitionPoints { get; set; }

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