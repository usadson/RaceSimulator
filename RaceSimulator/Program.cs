using Controller;
using RaceSimulator;
using System.Globalization;

var vCulture = new CultureInfo("it-IT");

Thread.CurrentThread.CurrentCulture = vCulture;
Thread.CurrentThread.CurrentUICulture = vCulture;
CultureInfo.DefaultThreadCurrentCulture = vCulture;
CultureInfo.DefaultThreadCurrentUICulture = vCulture;

I18N.Initialize();
Data.Initialize();

Application app = new ConsoleApplication();
app.InitializeData();
app.Run();
