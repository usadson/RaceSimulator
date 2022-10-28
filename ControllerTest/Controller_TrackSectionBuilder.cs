using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Controller;
using Model;

namespace ControllerTest;

[TestFixture]
internal class Controller_TrackSectionBuilder
{
    [Test]
    public void TestExceptionTurn()
    {
        TrackSectionsBuilder builder = new(Direction.North);
        Assert.Throws<InvalidOperationException>(() => builder.Turn(Direction.North));
    }

    class MockBuilder : TrackSectionsBuilder
    {
        public MockBuilder(Direction beginDirection) : base(beginDirection)
        {
        }

        public SectionTypes ProtectedCalculateSectionType(Direction from, Direction to)
        {
            return CalculateSectionType(from, to);
        }
    }

    [Test]
    public void TestException()
    {
        MockBuilder builder = new(Direction.North);
        Assert.Throws<InvalidDataException>(() => builder.ProtectedCalculateSectionType((Direction)600, Direction.North));

        Assert.That(builder.ProtectedCalculateSectionType(Direction.North, Direction.West), Is.EqualTo(SectionTypes.LeftCorner));
        Assert.That(builder.ProtectedCalculateSectionType(Direction.North, Direction.East), Is.EqualTo(SectionTypes.RightCorner));

        Assert.That(builder.ProtectedCalculateSectionType(Direction.South, Direction.West), Is.EqualTo(SectionTypes.RightCorner));
        Assert.That(builder.ProtectedCalculateSectionType(Direction.South, Direction.East), Is.EqualTo(SectionTypes.LeftCorner));

        Assert.That(builder.ProtectedCalculateSectionType(Direction.East, Direction.North), Is.EqualTo(SectionTypes.LeftCorner));
        Assert.That(builder.ProtectedCalculateSectionType(Direction.East, Direction.South), Is.EqualTo(SectionTypes.RightCorner));

        Assert.That(builder.ProtectedCalculateSectionType(Direction.West, Direction.North), Is.EqualTo(SectionTypes.RightCorner));
        Assert.That(builder.ProtectedCalculateSectionType(Direction.West, Direction.South), Is.EqualTo(SectionTypes.LeftCorner));
    }
}