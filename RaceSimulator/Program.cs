using Controller;
using RaceSimulator;

I18N.Initialize();
Data.Initialize();

const bool UseWindow = false;

Application app = UseWindow ? new ConsoleApplication() : new ConsoleApplication();
app.InitializeData();
app.Run();
