namespace GUIApplicationTests;

[TestFixture]
public sealed class ParticipantEntryTests
{
    private readonly Driver _participant = new(Character.BabyMario, 50, new Car(), TeamColors.Grey);

    [OneTimeSetUp]
    public void OneTimeSetup() => I18N.Initialize();

    [OneTimeTearDown]
    public void OneTimeTearDown() => I18N.Reset();

    [Test]
    public void VerifyPropertiesSet()
    {
        ParticipantEntry entry = new(_participant, false);
        Assert.That(entry.Participant, Is.SameAs(_participant));
        Assert.That(entry.Character, Is.EqualTo(_participant.Character));

        Assert.That(entry.Name, Is.EqualTo(I18N.Translate(_participant.Character.ToString())));
        Assert.That(entry.PositionInRace, Is.EqualTo(entry.Participant.PositionInRace));
        Assert.That(entry.PositionInCompetition, Is.EqualTo(entry.Participant.PositionInCompetition));

        Assert.That(entry.PictureRect.HasArea, Is.True);
        Assert.That(entry.PictureRect.X, Is.Positive);
        Assert.That(entry.PictureRect.Y, Is.Positive);

        Assert.That(entry.PositionRect.HasArea, Is.True);
        Assert.That(entry.PositionRect.X, Is.Positive);
        Assert.That(entry.PositionRect.Y, Is.Positive);

        Assert.That(entry.CompetitionPoints, Is.Not.Empty);
    }

    [Test]
    public void CompetitionPointsTest()
    {
        ParticipantEntry entry = new(_participant, false);

        _participant.CompetitionPoints = 1;
        Assert.That(entry.CompetitionPoints, Is.Not.Empty);

        // Value too high for an actual point calculation
        _participant.CompetitionPoints = int.MaxValue;
        Assert.That(entry.CompetitionPoints, Is.Not.Empty);
    }

    [Test]
    public void PositionRectRaceTest()
    {
        ParticipantEntry entry = new(_participant, false);

        _participant.PositionInRace = 1;
        Assert.That(entry.PositionRect.HasArea, Is.True);
        Assert.That(entry.PositionRect.X, Is.Positive);
        Assert.That(entry.PositionRect.Y, Is.Positive);

        // Value too high for an actual point calculation
        _participant.PositionInRace = ushort.MaxValue;
        Assert.That(entry.PositionRect.HasArea, Is.True);
        Assert.That(entry.PositionRect.X, Is.Positive);
        Assert.That(entry.PositionRect.Y, Is.Positive);
    }

    [Test]
    public void PositionRectCompetitionTest()
    {
        ParticipantEntry entry = new(_participant, true);

        _participant.CompetitionPoints = 1;
        Assert.That(entry.PositionRect.HasArea, Is.True);
        Assert.That(entry.PositionRect.X, Is.Positive);
        Assert.That(entry.PositionRect.Y, Is.Positive);

        // Value too high for an actual point calculation
        _participant.CompetitionPoints = ushort.MaxValue;
        Assert.That(entry.PositionRect.HasArea, Is.True);
        Assert.That(entry.PositionRect.X, Is.Positive);
        Assert.That(entry.PositionRect.Y, Is.Positive);
    }
}
