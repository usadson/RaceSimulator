using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Controller;
using Model;

namespace ControllerTest;

[TestFixture]
internal class Controller_SectionRegistry
{
    [TestCase(SectionTypes.Finish)]
    [TestCase(SectionTypes.LeftCorner)]
    [TestCase(SectionTypes.RightCorner)]
    [TestCase(SectionTypes.StartGrid)]
    [TestCase(SectionTypes.Straight)]
    public void TestContains(SectionTypes sectionType)
    {
        Assert.That(SectionRegistry.Lengths.ContainsKey(sectionType), Is.True);
    }

    [TestCase(SectionTypes.Finish)]
    [TestCase(SectionTypes.LeftCorner)]
    [TestCase(SectionTypes.RightCorner)]
    [TestCase(SectionTypes.StartGrid)]
    [TestCase(SectionTypes.Straight)]
    public void TestNonZero(SectionTypes sectionType)
    {
        Assert.That(SectionRegistry.Lengths[sectionType], Is.GreaterThan(0));
    }
}