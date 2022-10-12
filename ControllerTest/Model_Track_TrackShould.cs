using Model;

namespace ControllerTest;

[TestFixture]
public class Model_Track_TrackShould
{
    [Test]
    public void Construct()
    {
        Track track = new(Cup.Shell, "Luigi Circuit", Array.Empty<SectionTypes>(), 3);
        
        Assert.That(track.Cup, Is.EqualTo(Cup.Shell));
        Assert.That(track.Name, Is.EqualTo("Luigi Circuit"));
        Assert.That(track.Sections, Is.Empty);

        Section section = new();
        track.Sections.AddFirst(section);
        Assert.That(track.Sections, Has.Count.EqualTo(1));
        Assert.That(track.Sections.First(), Is.EqualTo(section));
    }
    
}