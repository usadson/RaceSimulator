using Model;

namespace ControllerTest;

[TestFixture]
public class Model_Section_Section
{
    [Test]
    public void Construct()
    {
        Section section = new()
        {
            SectionType = SectionTypes.Finish
        };
            
        Assert.That(section.SectionType, Is.EqualTo(SectionTypes.Finish));
        section.SectionType = SectionTypes.Straight;
        Assert.That(section.SectionType, Is.EqualTo(SectionTypes.Straight));
    }
    
}