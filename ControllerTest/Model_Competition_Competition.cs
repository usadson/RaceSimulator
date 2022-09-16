using Model;

namespace ControllerTest;

[TestFixture]
public class Model_Competition_Competition
{
    [Test]
    public void EmptyOnInit()
    {
        Competition competition = new();
        Assert.That(competition.Participants, Is.Empty);
        Assert.That(competition.Tracks, Is.Empty);
    }
    
    [Test]
    public void SettableAttributes()
    {
        List<IParticipant> participants = new()
        {
            new Driver(Character.Luigi, 0, new Car(), TeamColors.Blue)
        };

        Track track = new(Cup.Shell, "GCN Peach Beach", Array.Empty<Section>());

        Queue<Track> tracks = new();
        tracks.Enqueue(track);
        
        Competition competition = new();
        Assert.That(competition.Participants, Is.Empty);
        Assert.That(competition.Tracks, Is.Empty);

        competition.Participants = participants;
        competition.Tracks = tracks;
        Assert.That(competition.Participants, Is.EqualTo(participants));
        Assert.That(competition.Tracks, Is.EqualTo(tracks));
        
        Assert.That(competition.Tracks, Has.Count.EqualTo(1));
        Assert.That(competition.Tracks.First(), Is.EqualTo(track));
    }
}
