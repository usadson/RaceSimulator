using System.Drawing;
using System.Windows.Media.Imaging;

namespace GUIApplicationTests;

[TestFixture]
internal class RendererTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        I18N.Initialize();
        Data.Initialize();
        Data.NextRace();
        Assert.That(Data.HasRace(), Is.True);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Data.Reset();
        I18N.Reset();
    }

    [TearDown]
    public void TearDown() => SpriteManager.ClearCache();

    [Test]
    public void ConstructorTest()
    {
        var renderer = new Renderer(new(640, 480));
        Assert.That(renderer.FrameSize.IsEmpty, Is.False);
        Assert.That(renderer.FrameSize.Width, Is.EqualTo(640));
        Assert.That(renderer.FrameSize.Height, Is.EqualTo(480));
    }

    [Test]
    public void DisposeTest()
    {
        var renderer = new Renderer(new(640, 480));
        Assert.That(renderer, Is.InstanceOf<IDisposable>());
        renderer.Dispose();
    }

    [Test]
    public void DrawTest()
    {
        using var renderer = new Renderer(new(640, 480));
        var source = renderer.Draw();
        Assert.That(source, Is.Not.Null);
        Assert.That(source.Format.BitsPerPixel, Is.EqualTo(32));
        // verify if is ARGB?

        ulong pixelsNotBlack = 0;
        
        var stride = source.PixelWidth * 4;
        var array = new byte[stride * source.PixelHeight];
        source.CopyPixels(array, stride, 0);
        for (var y = 0; y < source.PixelHeight; ++y)
        {
            var xOffset = y * source.PixelWidth * 4;
            for (var x = 0; x < source.PixelWidth * 4; x += 4)
            {
                var alpha = array[xOffset + x];
                var red = array[xOffset + x + 1];
                var green = array[xOffset + x + 2];
                var blue = array[xOffset + x + 3];

                if (alpha != 0 && (red != 0 || green != 0 || blue != 0))
                {
                    ++pixelsNotBlack;
                }
            }
        }

#if save
        using (var stream = File.OpenWrite(@"C:\Users\tager\RenderTest.jpg"))
        {
            JpegBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(stream);
        }
#endif

        Assert.That(pixelsNotBlack, Is.GreaterThan(100));
    }
}