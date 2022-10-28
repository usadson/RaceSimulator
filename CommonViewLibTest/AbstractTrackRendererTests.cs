using System.Linq;

namespace CommonViewLibTest;

public class AbstractTrackRendererTests
{
    [SetUp]
    public void Setup()
    {
        Data.Initialize();
    }

    [TearDown]
    public void TearDown()
    {
        Data.Reset();
    }

    [Test]
    public void ConstructionTest()
    {
        var renderer = new MockTrackRenderer();
        Assert.That(renderer, Is.Not.Null);
        Assert.That(renderer, Is.InstanceOf(typeof(AbstractTrackRenderer)));
        Assert.That(renderer.GetProtected_CurrentSectionIndex, Is.Zero);
    }

    [Test]
    public void CoordinateTest()
    {
        var renderer = new MockTrackRenderer();
        Assert.That(renderer, Is.Not.Null);
        Assert.That(renderer.X, Is.Zero);
        Assert.That(renderer.Y, Is.Zero);

        renderer.X = 626.0f;
        renderer.Y = 999991.0f;

        Assert.That(renderer.CurrentPoint.X, Is.EqualTo(626.0f));
        Assert.That(renderer.CurrentPoint.Y, Is.EqualTo(999991.0f));
    }

    [Test]
    public void PointsTest()
    {
        var renderer = new MockTrackRenderer();
        Assert.That(renderer.BottomRightmostPoint.X, Is.Zero);
        Assert.That(renderer.BottomRightmostPoint.Y, Is.Zero);

        Assert.That(renderer.TopLeftmostPoint.X, Is.Zero);
        Assert.That(renderer.TopLeftmostPoint.Y, Is.Zero);

        renderer.SetPoints(new(100, 300), new(800, 500));
        Assert.That(renderer.TopLeftmostPoint.X, Is.EqualTo(100));
        Assert.That(renderer.TopLeftmostPoint.Y, Is.EqualTo(300));
        Assert.That(renderer.BottomRightmostPoint.X, Is.EqualTo(800));
        Assert.That(renderer.BottomRightmostPoint.Y, Is.EqualTo(500));
    }

    [Test]
    public void MockStartDrawTrackWithNullTrack()
    {
        Data.CurrentRace = null;
        Assert.That(Data.HasRace(), Is.False);

        var renderer = new MockTrackRenderer();

        Assert.That(renderer.GetProtected_CurrentSectionIndex, Is.Zero);
        renderer.MockStartDrawTrack();
        Assert.That(renderer.GetProtected_CurrentSectionIndex, Is.Zero);
    }

    [Test]
    public void MockStartDrawTrackWithEmptyTrack()
    {
        Data.CurrentRace =
            new Race(
                new Track(Data.CurrentCompetition!.Cup, "MockTrack", Array.Empty<SectionTypes>(), 1, Direction.East),
                new List<IParticipant>());

        var renderer = new MockTrackRenderer();

        Assert.That(renderer.GetProtected_CurrentSectionIndex, Is.Zero);
        renderer.MockStartDrawTrack();
        Assert.That(renderer.GetProtected_CurrentSectionIndex, Is.Zero);
    }

    private void SetTrackWithSectionTypes(SectionTypes[] sectionTypes, Direction beginDirection = Direction.East)
    {
        Data.CurrentRace =
            new Race(
                new Track(Data.CurrentCompetition!.Cup, "MockTrack", sectionTypes, 1, beginDirection),
                new List<IParticipant>());
    }

    [TestCase(SectionTypes.Finish)]
    [TestCase(SectionTypes.LeftCorner)]
    [TestCase(SectionTypes.RightCorner)]
    [TestCase(SectionTypes.StartGrid)]
    [TestCase(SectionTypes.Straight)]
    public void MockStartDrawTrackWithTrackWithSingleSection(SectionTypes sectionType)
    {
        SetTrackWithSectionTypes(new[] { sectionType });
        var renderer = new MockTrackRenderer();

        Assert.That(renderer.GetProtected_CurrentSectionIndex, Is.Zero);
        renderer.MockStartDrawTrack();
        Assert.That(renderer.GetProtected_CurrentSectionIndex, Is.EqualTo(1));
    }

    [Test]
    public void MockStartDrawTrackWithTrackWithABunchOfStraights()
    {
        var types = new SectionTypes[100];
        for (var i = 0; i < types.Length; ++i)
            types[i] = SectionTypes.Straight;

        SetTrackWithSectionTypes(types);
        var renderer = new MockTrackRenderer();

        Assert.That(renderer.GetProtected_CurrentSectionIndex, Is.Zero);
        renderer.MockStartDrawTrack();
        Assert.That(renderer.GetProtected_CurrentSectionIndex, Is.EqualTo(100));
    }

    [TestCase(SectionTypes.LeftCorner, Direction.North, Direction.West)]
    [TestCase(SectionTypes.LeftCorner, Direction.East, Direction.North)]
    [TestCase(SectionTypes.LeftCorner, Direction.South, Direction.East)]
    [TestCase(SectionTypes.LeftCorner, Direction.West, Direction.South)]

    [TestCase(SectionTypes.RightCorner, Direction.North, Direction.East)]
    [TestCase(SectionTypes.RightCorner, Direction.East, Direction.South)]
    [TestCase(SectionTypes.RightCorner, Direction.South, Direction.West)]
    [TestCase(SectionTypes.RightCorner, Direction.West, Direction.North)]
    public void TestMoveCursor(SectionTypes sectionType, Direction beginDirection, Direction endDirection)
    {
        SetTrackWithSectionTypes(new []{sectionType}, beginDirection);
        var renderer = new MockTrackRenderer();

        Assert.That(renderer.GetLastDirectionUpdate(), Is.Null);
        renderer.MockStartDrawTrack();
        Assert.That(renderer.GetProtected_CurrentSectionIndex, Is.EqualTo(1));

        var lastDirectionUpdate = renderer.GetLastDirectionUpdate();
        Assert.That(lastDirectionUpdate, Is.Not.Null);
        Assert.That(lastDirectionUpdate.HasValue, Is.True);
        Assert.That(lastDirectionUpdate.Value.Item1, Is.EqualTo(beginDirection));
        Assert.That(lastDirectionUpdate.Value.Item2, Is.EqualTo(endDirection));
    }

    [Test]
    public void TestMoveCursorCircle()
    {
        SetTrackWithSectionTypes(new[]
        {
            SectionTypes.LeftCorner,
            SectionTypes.LeftCorner,
            SectionTypes.LeftCorner,
            SectionTypes.LeftCorner,
            SectionTypes.RightCorner,
            SectionTypes.RightCorner,
            SectionTypes.RightCorner,
            SectionTypes.RightCorner
        });
        var renderer = new MockTrackRenderer();

        Assert.That(renderer.GetLastDirectionUpdate(), Is.Null);
        renderer.MockStartDrawTrack();
        Assert.That(renderer.GetProtected_CurrentSectionIndex, Is.EqualTo(8));
    }

    [Test]
    public void TestUpdateOutermostPointsBigger()
    {
        var types = new SectionTypes[100];
        for (var i = 0; i < types.Length; ++i)
            types[i] = SectionTypes.Straight;

        SetTrackWithSectionTypes(types);
        var renderer = new MockTrackRenderer
        {
            X = 100000,
            Y = 100000
        };

        Assert.That(renderer.GetProtected_CurrentSectionIndex, Is.Zero);
        renderer.MockStartDrawTrack();
        Assert.That(renderer.GetProtected_CurrentSectionIndex, Is.EqualTo(100));
    }

    [Test]
    public void TestUpdateOutermostPointsSmaller()
    {
        var types = new SectionTypes[100];
        for (var i = 0; i < types.Length; ++i)
            types[i] = SectionTypes.Straight;

        SetTrackWithSectionTypes(types);
        var renderer = new MockTrackRenderer
        {
            X = -100000,
            Y = -100000
        };

        Assert.That(renderer.GetProtected_CurrentSectionIndex, Is.Zero);
        renderer.MockStartDrawTrack();
        Assert.That(renderer.GetProtected_CurrentSectionIndex, Is.EqualTo(100));
    }
}