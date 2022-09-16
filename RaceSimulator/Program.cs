
using RaceSimulator;
using SFML;
using SFML.Graphics;
using SFML.Window;
using Model;
using Controller;
using SFML.System;

RenderWindow window = new(new VideoMode(1280, 720), "Race Simulator");
Data.Initialize();
Data.NextRace();

Font font = new("C:\\Windows\\Fonts\\segoeui.ttf");

Text text = new("", font)
{
    CharacterSize = 20,
    FillColor = Color.White,
    OutlineColor = Color.Black,
    OutlineThickness = 1
};

void DrawHUD()
{
    Vector2f offset = new(10, 10);

    text.Position = new(offset.X, offset.Y);
    text.DisplayedString = "Track: ";
    text.Style = Text.Styles.Bold;
    window.Draw(text);

    text.Position = new(offset.X + text.GetGlobalBounds().Width, offset.Y);
    text.DisplayedString = Data.CurrentRace.Track.Name;
    text.Style = Text.Styles.Italic;
    window.Draw(text);

    var bounds = text.GetGlobalBounds();
    text.Position = new(offset.X, bounds.Top + bounds.Height);
    text.DisplayedString = "Participants: ";
    text.Style = Text.Styles.Bold;
    window.Draw(text);

    text.Position = new(offset.X + text.GetGlobalBounds().Width, text.Position.Y);
    text.DisplayedString = string.Join(", ", Data.CurrentRace.Participants.ConvertAll(x => x.Name));    
    text.Style = Text.Styles.Italic;
    window.Draw(text);
}

while (window.IsOpen)
{
    window.DispatchEvents();

    window.Clear(new Color(71, 193, 0));

    DrawHUD();

    window.Display();
}

window.Dispose();
