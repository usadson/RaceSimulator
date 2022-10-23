namespace Model
{
    public class Car : IEquipment
    {
        public int Quality { get; set; }
        public int Performance { get; set; }
        public int Speed { get; set; }
        public bool IsBroken { get; set; } = false;
        public DateTime? WhenWasBroken { get; set; }

        public Car()
        {
            Random rand = new();
            Speed = rand.Next(4, 6);
            Performance = rand.Next(4, 6);
            Quality = rand.Next(60, 100);
        }
    }
}
