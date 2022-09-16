using Model;

namespace ControllerTest;

[TestFixture]
public class Model_Car_Car
{
    [Test]
    public void Construct()
    {
        Car car = new();

        Assert.That(car.Performance, Is.EqualTo(0));
        Assert.That(car.Quality, Is.EqualTo(0));
        Assert.That(car.Speed, Is.EqualTo(0));
        Assert.That(car.IsBroken, Is.EqualTo(false));
    }

    [Test]
    public void ConstructWithInputs()
    {
        Car car = new()
        {
            Performance = 1,
            Quality = 62,
            Speed = 100,
            IsBroken = true
        };
        
        Assert.That(car.Performance, Is.EqualTo(1));
        Assert.That(car.Quality, Is.EqualTo(62));
        Assert.That(car.Speed, Is.EqualTo(100));
        Assert.That(car.IsBroken, Is.EqualTo(true));
    }

    [Test]
    public void ChangeableTypes()
    {
        Car car = new()
        {
            Performance = 626,
            Quality = 26,
            Speed = 99,
            IsBroken = true
        };
        
        Assert.That(car.Performance, Is.EqualTo(626));
        car.Performance = 16;
        Assert.That(car.Performance, Is.EqualTo(16));
        
        Assert.That(car.Quality, Is.EqualTo(26));
        car.Quality = 78;
        Assert.That(car.Quality, Is.EqualTo(78));
        
        Assert.That(car.Speed, Is.EqualTo(99));
        car.Speed = 68;
        Assert.That(car.Speed, Is.EqualTo(68));
        
        Assert.That(car.IsBroken, Is.EqualTo(true));
        car.IsBroken = false;
        Assert.That(car.IsBroken, Is.EqualTo(false));
    }
    
}