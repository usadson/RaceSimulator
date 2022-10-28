using Model;

namespace ControllerTest;

[TestFixture]
internal class Model_IParticipantCompetitionComparer
{
    private readonly IParticipantCompetitionComparer _comparer = new();

    [Test]
    public void TestNoParticipant()
    {
        List<IParticipant> participants = new();
        var sorted = new List<IParticipant>(participants);
        sorted.Sort(_comparer);
        Assert.That(sorted, Is.EquivalentTo(participants));
    }

    [Test]
    public void TestSingleParticipant()
    {
        List<IParticipant> participants = new()
        {
            new Driver(Character.Toad, 100, new Car(), TeamColors.Green)
        };
        var sorted = new List<IParticipant>(participants);
        sorted.Sort(_comparer);
        Assert.That(sorted, Is.EquivalentTo(participants));
    }

    [Test]
    public void TestSingleNormal()
    {
        List<IParticipant> participants = new()
        {
            new Driver(Character.Toad, 100, new Car(), TeamColors.Green),
            new Driver(Character.BabyDaisy, 200, new Car(), TeamColors.Blue)
        };

        Competition competition = new(Cup.Banana);
        competition.Participants = participants;

        // Competition not started, so value must be 12.
        Assert.That(participants[0].PositionInCompetition, Is.EqualTo(12));
        Assert.That(participants[1].PositionInCompetition, Is.EqualTo(12));

        participants[0].OnNewRace(competition);
        participants[1].OnNewRace(competition);

        var sorted = new List<IParticipant>(participants);
        sorted.Sort(_comparer);
        Assert.That(sorted, Is.EquivalentTo(participants));
    }

    [Test]
    public void TestRawNullables()
    {
        Assert.That(_comparer.Compare(null, null), Is.EqualTo(0));

        IParticipant participant = new Driver(Character.Birdo, 345, new Car(), TeamColors.Yellow);

        Assert.That(_comparer.Compare(null, participant), Is.EqualTo(1));
        Assert.That(_comparer.Compare(participant, null), Is.EqualTo(-1));

        Assert.That(_comparer.Compare(participant, participant), Is.EqualTo(0));
    }
}