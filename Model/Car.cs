namespace Model
{
    public class Car : IEquipment
    {
        public int Quality { get; set; } = 100;
        public int Performance { get; set; } = 2;
        public int Speed { get; set; } = 4;
        public bool IsBroken { get; set; } = false;

        public Car()
        {
            Random rand = new();
            Speed = rand.Next(6, 16);
            Performance = rand.Next(4, 8);
            Quality = rand.Next(60, 100);
        }
    }
}
