using Controller;

namespace ControllerTest;

[TestFixture]
public class Controller_Data_Initialize
{
    [Test]
    public void Initialize()
    {
        Data.Initialize();
    }

    [TearDown]
    public void TearDown()
    {
        Data.Reset();
    }

    [Test]
    public void CreateResourcesTest()
    {
        Assert.That(Data.CreateResources(), Is.Not.Null);
    }
    
}