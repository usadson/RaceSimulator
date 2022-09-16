namespace Model
{
    public interface IParticipant
    {
        public string Name { get; }
        public int Points { get; set; }
        public IEquipment Equipment { get; set; }
        public TeamColors TeamColor { get; }
    }
}
