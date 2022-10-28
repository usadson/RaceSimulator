namespace GUIApplicationTests;

[TestFixture]
[Apartment(ApartmentState.STA)]
internal class MainWindowTests
{
    private readonly App _app = new();

#if disabled
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Assert.That(Application.ResourceAssembly, Is.Null);
        Application.ResourceAssembly = typeof(MainWindow).Assembly;
    }
#endif

    [Test, STAThread]
    public void ConstructorTest()
    {
        var wait = new AutoResetEvent(false);
        
        Data.NewRaceStarting += _ => wait.Set();

        var window = new MainWindow();
        Assert.That(I18N.IsInitialized, Is.True);
        Assert.That(Data.CurrentCompetition, Is.Not.Null);

        Assert.IsTrue(wait.WaitOne(TimeSpan.FromSeconds(5)));
        Assert.That(Data.HasRace(), Is.True);
        
        window.OnClosed();
    }

}