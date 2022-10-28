using Model;

namespace ControllerTest;

[TestFixture]
internal class Model_Bounds
{
    [Test]
    public void TestConstructor()
    {
        var bounds = new Bounds(-148, 5, 100, 200);
        Assert.That(bounds.X, Is.EqualTo(-148));
        Assert.That(bounds.Y, Is.EqualTo(5));
        Assert.That(bounds.Width, Is.EqualTo(100));
        Assert.That(bounds.Height, Is.EqualTo(200));

        Assert.That(bounds.Left, Is.EqualTo(bounds.X));
        Assert.That(bounds.Top, Is.EqualTo(bounds.Y));
        Assert.That(bounds.Right, Is.EqualTo(bounds.X + bounds.Width));
        Assert.That(bounds.Bottom, Is.EqualTo(bounds.Y + bounds.Height));

        Assert.That(bounds.MiddleX, Is.EqualTo(bounds.Left + (bounds.Right - bounds.Left) / 2));
        Assert.That(bounds.MiddleY, Is.EqualTo(bounds.Top + (bounds.Bottom - bounds.Top) / 2));
    }

    [Test]
    public void TestCreateWithTopBottom()
    {
        var bounds = Bounds.CreateWithLeftTopRightBottom(52, 67, 529, 865);
        Assert.That(bounds.X, Is.EqualTo(52));
        Assert.That(bounds.Y, Is.EqualTo(67));
        Assert.That(bounds.Width, Is.EqualTo(bounds.Right - bounds.X));
        Assert.That(bounds.Height, Is.EqualTo(bounds.Bottom - bounds.Y));

        Assert.That(bounds.Left, Is.EqualTo(bounds.X));
        Assert.That(bounds.Top, Is.EqualTo(bounds.Y));
        Assert.That(bounds.Right, Is.EqualTo(529));
        Assert.That(bounds.Bottom, Is.EqualTo(865));
    }
}