using Controller;

namespace ControllerTest;

[TestFixture]
public class Controller_Data_NextRace
{
    [SetUp]
    public void Initialize()
    {
        Data.Initialize();
    }

    [Test]
    public void Default()
    {
        Assert.That(Data.CurrentCompetition, Is.Not.Null);
        var firstTrack = Data.CurrentCompetition.Tracks.Peek();
        
        Assert.That(Data.HasRace(), Is.False);
        
        Data.NextRace();
        Assert.That(Data.HasRace(), Is.True);
        Assert.That(Data.CurrentRace.Track, Is.EqualTo(firstTrack));
    } 

    [TearDown]
    public void TearDown()
    {
        Data.Reset();
    }
    
}