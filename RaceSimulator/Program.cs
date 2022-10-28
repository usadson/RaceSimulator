using Controller;
using RaceSimulator;

I18N.Initialize();
Data.Initialize();

Application app = new ConsoleApplication();
app.InitializeData();
app.Run();
