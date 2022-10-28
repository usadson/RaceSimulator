using System.Drawing;
using System.Drawing.Imaging;

namespace GUIApplicationTests;

[TestFixture]
internal class SpriteManagerTests
{
    [SetUp]
    public void SetUp() => SpriteManager.ClearCache();

    [TestCase(1, 1)]
    [TestCase(480, 640)]
    [TestCase(640, 480)]
    public void GetEmptyBitmap(int width, int height)
    {
        var bitmap = SpriteManager.GetEmptyBitmap(width, height);
        Assert.That(bitmap.Width, Is.EqualTo(width));
        Assert.That(bitmap.Height, Is.EqualTo(height));

        Assert.That(bitmap.PixelFormat, Is.EqualTo(PixelFormat.Format32bppArgb));

        var emptyColor = Color.FromArgb(0, 0, 0, 0);
        for (var x = 0; x < width; ++x)
        {
            for (var y = 0; y < height; ++y)
            {
                Assert.That(bitmap.GetPixel(x, y), Is.EqualTo(emptyColor));
            }
        }
    }

}