using Model;
using System.ComponentModel;

namespace ControllerTest;

[TestFixture]
internal class Models_Directions
{
    [TestCase(Direction.North, Direction.West)]
    [TestCase(Direction.West, Direction.South)]
    [TestCase(Direction.South, Direction.East)]
    [TestCase(Direction.East, Direction.North)]
    public void TestRotateLeft(Direction input, Direction result)
    {
        Assert.That(Directions.RotateLeft(input), Is.EqualTo(result));
    }

    [TestCase(Direction.North, Direction.East)]
    [TestCase(Direction.East, Direction.South)]
    [TestCase(Direction.South, Direction.West)]
    [TestCase(Direction.West, Direction.North)]
    public void TestRotateRight(Direction input, Direction result)
    {
        Assert.That(Directions.RotateRight(input), Is.EqualTo(result));
    }

    [TestCase(Direction.North, Direction.South)]
    [TestCase(Direction.East, Direction.West)]
    [TestCase(Direction.South, Direction.North)]
    [TestCase(Direction.West, Direction.East)]
    public void TestOpposite(Direction input, Direction result)
    {
        Assert.That(Directions.Opposite(input), Is.EqualTo(result));
    }

    [Test]
    public void TestExceptions()
    {
        Assert.Throws<InvalidEnumArgumentException>(() => Directions.Opposite((Direction)10000));
        Assert.Throws<InvalidEnumArgumentException>(() => Directions.RotateLeft((Direction)10000));
        Assert.Throws<InvalidEnumArgumentException>(() => Directions.RotateRight((Direction)10000));
    }
}