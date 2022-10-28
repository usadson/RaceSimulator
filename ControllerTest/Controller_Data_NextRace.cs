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
    public void CurrentRaceTest()
    {
        Data.Reset();

        Assert.Throws<NullReferenceException>(() => Data.CurrentRace.Dispose());
    }

    [Test]
    public void Default()
    {
        Assert.That(Data.CurrentCompetition, Is.Not.Null);
        Assert.That(Data.HasNextRace, Is.True);

        var firstTrack = Data.CurrentCompetition!.Tracks.Peek();
        
        Assert.That(Data.HasRace(), Is.False);
        
        Data.NextRace();
        Assert.That(Data.HasRace(), Is.True);
        Assert.That(Data.CurrentRace.Track, Is.EqualTo(firstTrack));

        while (Data.HasNextRace)
        {
            Data.NextRace();
        }
        Data.NextRace();

        Assert.That(Data.HasNextRace, Is.False);
        Assert.That(Data.HasRace, Is.False);
    } 

    [TearDown]
    public void TearDown()
    {
        Data.Reset();
    }
    
}