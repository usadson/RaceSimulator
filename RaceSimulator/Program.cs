
using RaceSimulator;
using SFML;
using SFML.Graphics;
using SFML.Window;

RenderWindow window = new RenderWindow(new VideoMode(1280, 720), "Race Simulator"); 

while (window.IsOpen)
{
    window.DispatchEvents();

    window.Clear(new Color(71, 193, 0));


    window.Display();
}

window.Dispose();
