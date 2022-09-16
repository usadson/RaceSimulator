using Model;

namespace ControllerTest;

[TestFixture]
public class Model_Driver_Driver
{
    [Test]
    public void Construct()
    {
        Driver driver = new(Character.Rosalina, 100, new Car(), TeamColors.Blue);

        Assert.That(driver.Character, Is.EqualTo(Character.Rosalina));
        Assert.That(driver.Name, Is.EqualTo(Character.Rosalina.ToString()));
        Assert.That(driver.Points, Is.EqualTo(100));
        Assert.That(driver.Equipment, Is.InstanceOf<Car>());
        Assert.That(driver.TeamColor, Is.EqualTo(TeamColors.Blue));
    }

    [Test]
    public void ChangeableTypes()
    {
        Driver driver = new(Character.KingBoo, 50, new Car(), TeamColors.Red);
        
        Assert.That(driver.Points, Is.EqualTo(50));
        driver.Points = 100;
        Assert.That(driver.Points, Is.EqualTo(100));

        Assert.That(((Car)driver.Equipment).Performance, Is.EqualTo(0));
        driver.Equipment = new Car
        {
            Performance = 14
        };
        Assert.That(((Car)driver.Equipment).Performance, Is.EqualTo(14));
    }
    
}