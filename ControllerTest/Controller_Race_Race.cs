using Controller;
using Model;

namespace ControllerTest;

[TestFixture]
public class Controller_Race_Race
{
    private readonly Track _track = new(Cup.Banana, "N64 Sherbet Island", 
        new SectionTypes[]
        {
            SectionTypes.StartGrid
        },
        3
    );

    private readonly List<IParticipant> _participants = new()
    {
        new Driver(Character.KoopaTroopa, 0, new Car(), TeamColors.Yellow)
    };
    
    [Test]
    public void Construct()
    {
        Race race = new(_track, _participants);
        Assert.That(DateTime.Now.Millisecond - race.StartTime.Millisecond, Is.LessThan(5));
        Assert.That(race.Participants, Is.EqualTo(_participants));
        Assert.That(race.Track, Is.EqualTo(_track));
    }
    
}