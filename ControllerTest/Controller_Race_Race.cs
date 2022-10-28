using Controller;
using Model;

namespace ControllerTest;

[TestFixture]
public class Controller_Race_Race
{
    private readonly Track _track = new(Cup.Banana, "N64 Sherbet Island",
        new[]
        {
            SectionTypes.StartGrid
        },
        3
    );

    private readonly List<IParticipant> _participants = new()
    {
        new Driver(Character.KoopaTroopa, 0, new Car(), TeamColors.Yellow)
    };

    private SectionTypes[] RandomizedSectionTypes(int max = 50)
    {
        var random = new Random();
        var result = new SectionTypes[random.Next(4, max)];
        for (var i = 0; i < result.Length; ++i)
            result[i] = (SectionTypes)random.Next(0, 6);
        return result;
    }

    private void MockDriversChanged(Race sender, DriversChangedEventArgs e)
    {
    }

    private void MockParticipantsOrderModified(Race sender)
    {
    }

    private void MockGameFinished(Race sender)
    {
    }

    private void MockParticipantLapped(Race sender, IParticipant participant, bool finished)
    {
    }

    [SetUp]
    public void SetUp() => Data.Initialize();

    [TearDown]
    public void TearDown() => Data.Reset();

    [Test]
    public void SelfTest()
    {
        using Race race = new(_track, _participants);
        MockDriversChanged(race, new(race.Track));
        MockParticipantsOrderModified(race);
        MockGameFinished(race);
        MockParticipantLapped(race, race.Participants[0], false);
    }

    [Test]
    public void Construct()
    {
        Race race = new(_track, _participants);
        Assert.That(DateTime.Now.Millisecond - race.StartTime.Millisecond, Is.LessThan(20));
        Assert.That(race.Participants, Is.EqualTo(_participants));
        Assert.That(race.Track, Is.EqualTo(_track));
    }

    [Test]
    public void TestNotifyAllChanged()
    {
        Race race = new(new Track(Cup.Shell, "MockTrackNotifyAllChanged", RandomizedSectionTypes(), 3), _participants);
        foreach (var section in race.Track.Sections)
        {
            var sectionData = race.GetSectionData(section);
            
            Assert.That(sectionData.Changed, Is.True);
            sectionData.Changed = false;
        }

        race.NotifyAllChanged();
        // Positions aren't committed by NotifyAllChanged, so nothing should've been changed.
        foreach (var section in race.Track.Sections)
        {
            Assert.That(race.GetSectionData(section).Changed, Is.False);
        }

        race.CommitPositions();

        foreach (var section in race.Track.Sections)
        {
            var sectionData = race.GetSectionData(section);
            Assert.That(sectionData.Changed, Is.True);
        }
    }

    [Test]
    public void DisposesCorrectly()
    {
        Race race = new(_track, _participants);
        Assert.That(race.DriversChangedEventListenerCount, Is.Zero);
        Assert.That(race.GameFinishedEventListenerCount, Is.Zero);
        Assert.That(race.GameFinishedEventListenerCount, Is.Zero);
        Assert.That(race.ParticipantsLappedEventListenerCount, Is.Zero);

        Assert.That(race.DriversChangedEventListenerCount, Is.Zero);
        race.DriversChanged += MockDriversChanged;
        Assert.That(race.DriversChangedEventListenerCount, Is.EqualTo(1));

        Assert.That(race.ParticipantsOrderModifiedEventListenerCount, Is.Zero);
        race.ParticipantsOrderModified += MockParticipantsOrderModified;
        Assert.That(race.ParticipantsOrderModifiedEventListenerCount, Is.EqualTo(1));

        Assert.That(race.GameFinishedEventListenerCount, Is.Zero);
        race.GameFinished += MockGameFinished;
        Assert.That(race.GameFinishedEventListenerCount, Is.EqualTo(1));

        Assert.That(race.ParticipantsLappedEventListenerCount, Is.Zero);
        race.ParticipantLapped += MockParticipantLapped;
        Assert.That(race.ParticipantsLappedEventListenerCount, Is.EqualTo(1));

        race.Dispose();
        Assert.That(race.DriversChangedEventListenerCount, Is.Zero);
        Assert.That(race.GameFinishedEventListenerCount, Is.Zero);
        Assert.That(race.GameFinishedEventListenerCount, Is.Zero);
        Assert.That(race.ParticipantsLappedEventListenerCount, Is.Zero);
    }

    [Test]
    public void TestStart()
    {
        using Race race = new(_track, _participants);
        Assert.That(race.IsRunning, Is.False);
        race.Start();
        Assert.That(race.IsRunning, Is.True);
    }

    [Test, Timeout(500)]
    public void TestParticipantsAreSorted()
    {
        Track track = new(Cup.Shell, "Coopa Cape", new[]
        {
            SectionTypes.Straight,
            SectionTypes.Straight,
            SectionTypes.Straight,
            SectionTypes.Straight,
            SectionTypes.Finish
        }, 50);
        
        Race race = new(track, new List<IParticipant>
        {
            new Driver(Character.Luigi, 100, new Car(), TeamColors.Blue),
            new Driver(Character.KoopaTroopa, 100, new Car(), TeamColors.Green)
        });

        race.Participants[1].Equipment.Performance = 10000000;
        race.PulseRun = true;

        race.DriversChanged += (_, _) => race.Dispose();

        race.Start();

        while (race.IsRunning)
        {
            Thread.Yield();
        }
    }

    [Test]
    public void TestDriversChangedEventArgs()
    {
        var args = new DriversChangedEventArgs(_track);
        Assert.That(args.Track, Is.SameAs(_track));
    }
}