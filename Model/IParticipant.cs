namespace Model
{
    public interface IParticipant
    {
        public string Name { get; }
        public int Points { get; set; }
        public IEquipment Equipment { get; set; }
        public TeamColors TeamColor { get; }

        public Section? CurrentSection { get; set; }
        public int DistanceFromStart { get; set; }
        public int Lap { get; set; }
        public int? Ranking { get; set; }
        public string LapStringified { get; }
        public TimeSpan? Time { get; set; }
        public int OverallRanking { get; set; }
        public ushort PositionInRace { get; set; }

        public void OnNewRace();
    }
}
