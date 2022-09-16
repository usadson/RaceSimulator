using Controller;

namespace ControllerTest;

[TestFixture]
public class Controller_TrackRegistry_Initialize
{
    [Test]
    public void Initialize()
    {
        TrackRegistry.Initialize();
        
        Assert.That(TrackRegistry.All, Has.Count.EqualTo(4));
    }

    [TearDown]
    public void TearDown()
    {
        TrackRegistry.Reset();
    }

}