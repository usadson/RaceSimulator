using RaceSimulator;
using SFML;
using SFML.Graphics;
using SFML.Window;
using Model;
using Controller;
using SFML.System;
using System.Diagnostics;
using System;

namespace RaceSimulator
{
    internal class WindowedApplication : Application
    {
        private Sprite sprite;

        private readonly Dictionary<SectionTypes, Sprite> sectionSprites = new()
        {
            { SectionTypes.Straight, new(new Texture("C:\\Data\\MKWii\\Road_01_Tile_04.png")) },
            { SectionTypes.RightCorner, new(new Texture("C:\\Data\\MKWii\\Road_01_Tile_02.png")) },
            { SectionTypes.Finish, new(new Texture("C:\\Data\\MKWii\\Road_01_Tile_03.png")) },
        };

        private Dictionary<SectionTypes, IntRect> sectionSizeOverrides = new()
        {
            { SectionTypes.RightCorner, new() }
        };

        private const int iconSize = 29;
        private Font font = new("C:\\Windows\\Fonts\\segoeui.ttf");
        private RectangleShape hudOverlay;
        private Text text, nameText;

        private RenderWindow window;
        private int scalingIndex;

        private readonly float[] scalings = new float[]
        {
            0.001f, 0.0025f, 0.005f, 0.0075f,
            0.01f, 0.025f, 0.05f, 0.075f,
            0.1f, 0.25f, 0.5f, 0.75f,
            1,
            1.25f, 1.5f,
            1.75f, 2.0f,
            2.25f, 2.5f,
        };

        public WindowedApplication()
        {
            window = window = new(new VideoMode(1280, 720), "Carro di Mario"); 
            text = new("", font)
            {
                CharacterSize = 20,
                FillColor = Color.White,
                OutlineColor = Color.Black,
                OutlineThickness = 1
            };

            nameText = new("", font)
            {
                CharacterSize = 12,
                FillColor = Color.White,
                Style = Text.Styles.Bold,
            };

            hudOverlay = new()
            {
                FillColor = new(0, 0, 0, 100),
                Size = new(window.Size.X, 100),
            };

            sprite = new()
            {
                Texture = new("C:\\Data\\MKWii\\minimap_icons.png"),
            };

            scalingIndex = scalings.Select((f, index) => (f, index)).First(p => p.f == 0.1f).index;
        }


        public override void Run()
        {
            SetWindowIcon();

            window.Closed += (e, s) => window.Close();
            window.KeyReleased += (e, s) =>
            {
                switch (s.Code)
                {
                    case Keyboard.Key.Num1:
                        Options.RenderTrackSectionBeforeRotationOverlay = !Options.RenderTrackSectionBeforeRotationOverlay;
                        break;
                    case Keyboard.Key.Num2:
                        Options.RenderTrackSectionBeginPosition = !Options.RenderTrackSectionBeginPosition;
                        break;
                    case Keyboard.Key.Add:
                        scalingIndex = Math.Min(scalingIndex + 1, scalings.Length - 1);
                        break;
                    case Keyboard.Key.Subtract:
                        scalingIndex = Math.Max(scalingIndex - 1, 0);
                        break;
                    default:
                        break;
                }
            };

            while (window.IsOpen)
            {
                window.DispatchEvents();

                Update();

                window.Clear(new Color(71, 193, 0));

                DrawTrack(SectionTypes.RightCorner);
                DrawTrack(SectionTypes.Straight);
                DrawHUD();

                window.Display();
            }

            window.Dispose();
        }

        private float DrawPlayerIcon(Character character, float x, float y)
        {
            if (SpriteOffsets.MinimapOffsets.TryGetValue(character, out IntRect offsets))
            {
                sprite.TextureRect = offsets;
            }

            string name = I18N.Translate(character.ToString());
            string[] parts = name.Split(' ');
            float biggestNamePart = 0;

            foreach (string part in parts)
            {
                nameText.DisplayedString = part;
                var width = nameText.GetGlobalBounds().Width;
                if (biggestNamePart < width)
                    biggestNamePart = width;
            }

            var spriteBounds = sprite.GetGlobalBounds();
            biggestNamePart = (biggestNamePart - spriteBounds.Width) / 2;

            sprite.Position = new(x + biggestNamePart, y);
            window.Draw(sprite);
            spriteBounds = sprite.GetGlobalBounds();

            float minX = spriteBounds.Left;
            float maxX = minX + spriteBounds.Width;

            float yOffset = spriteBounds.Top + spriteBounds.Height;
            foreach (string part in parts)
            {
                nameText.DisplayedString = part;
                var textBounds = nameText.GetGlobalBounds();

                nameText.Position = new(
                    spriteBounds.Left + (spriteBounds.Width - textBounds.Width) / 2,
                    yOffset
                );

                textBounds = nameText.GetGlobalBounds();
                float right = textBounds.Left + textBounds.Width;
                if (maxX < right)
                    maxX = right;
                if (minX > textBounds.Left)
                    minX = textBounds.Left;

                window.Draw(nameText);

                yOffset += textBounds.Height * 1.2f;
            }

            return maxX - minX;
        }

        void DrawHUD()
        {
            window.Draw(hudOverlay);
            Vector2f offset = new(10, 10);

            text.Position = new(offset.X, offset.Y);
            text.DisplayedString = "Pista di Gara: ";
            text.Style = Text.Styles.Bold;
            window.Draw(text);

            text.Position = new(offset.X + text.GetGlobalBounds().Width, offset.Y);
            text.DisplayedString = I18N.Translate(Data.CurrentRace.Track.Name);
            text.Style = Text.Styles.Italic;
            window.Draw(text);

            var bounds = text.GetGlobalBounds();
            text.Position = new(offset.X, bounds.Top + bounds.Height);
            text.DisplayedString = "Partecipanti: ";
            text.Style = Text.Styles.Bold;
            window.Draw(text);


            sprite.Position = new(text.Position.X + text.GetGlobalBounds().Width, text.Position.Y);
            var x = sprite.Position.X;
            foreach (IParticipant participant in Data.CurrentRace.Participants)
            {
                var driver = (Driver)participant;
                var y = sprite.Position.Y;

                x += DrawPlayerIcon(driver.Character, x, y) + 10;
            }

            //    text.Position = new(offset.X + text.GetGlobalBounds().Width, text.Position.Y);
            //    text.DisplayedString = string.Join(", ", Data.CurrentRace.Participants.ConvertAll(x => x.Name));    
            //    text.Style = Text.Styles.Italic;
            //    window.Draw(text);
        }

        RectangleShape rectShape = new();

        private void DrawTrack(SectionTypes? specific)
        {
            float scaling = scalings[scalingIndex];

            Vector2f startPos = new(200, hudOverlay.Size.Y * 4);
            Vector2f pos = new(startPos.X, startPos.Y);


            Vector2f scale = new(scaling, scaling);

            var dirs = new Vector2f[]
            {
                new(0, -1), // north
                new(1, 0), // east
                new(0, 1), // south
                new(-1, 0), // west
            };
            int newDir = 0;

            int index = 0;
            foreach (Section section in Data.CurrentRace.Track.Sections)
            {
                ++index;
                int oldDir = newDir;

                var sprite = sectionSprites[section.SectionType];

                sprite.Scale = scale;
                sprite.Origin = new(sprite.TextureRect.Width / 2, sprite.TextureRect.Height / 2);
                sprite.Position = new(pos.X + sprite.TextureRect.Width / 2 * scaling, pos.Y - sprite.TextureRect.Height / 2 * scaling);

                var boundsBeforeRotation = sprite.GetGlobalBounds();

                sprite.Rotation = oldDir * 90;

                switch (section.SectionType)
                {
                    case SectionTypes.Straight:
                        sprite.Rotation = (oldDir + 1) * 90;
                        break;
                    case SectionTypes.RightCorner:
                        newDir = (oldDir + 1) % 4;
                        break;
                    default:
                        break;
                }

                var bounds = sprite.GetGlobalBounds();
                var xOffset = bounds.Width / 2;
                var yOffset = bounds.Height / 2;
                sprite.Position = new(pos.X + xOffset, pos.Y - yOffset);

                if (specific == null || section.SectionType == specific)
                {
                    window.Draw(sprite);

                    if (Options.RenderTrackSectionBeforeRotationOverlay)
                    {
                        if (section.SectionType == SectionTypes.Straight)
                            rectShape.FillColor = new Color(255, 0, 0, 100);
                        else
                            rectShape.FillColor = new Color(0, 0, 255, 100);

                        rectShape.Size = new(boundsBeforeRotation.Width, boundsBeforeRotation.Height);
                        rectShape.Position = new(boundsBeforeRotation.Left, boundsBeforeRotation.Top);
                        window.Draw(rectShape);
                    }

                    if (Options.RenderTrackSectionBeginPosition)
                    {
                        rectShape.Position = new(pos.X - 1, pos.Y - 1);
                        if (index == 1)
                            rectShape.FillColor = Color.Red;
                        else
                            rectShape.FillColor = Color.Yellow;
                        rectShape.Size = new(3, 3);
                        rectShape.Scale = new(1, 1);
                        window.Draw(rectShape);
                    }
                }

                bounds = sprite.GetGlobalBounds();
                pos.X += dirs[newDir].X * bounds.Width;
                pos.Y += dirs[newDir].Y * bounds.Height;
            }

#if DEBUG_OVERLAY
    rectShape.Position = new(200, 200);
    rectShape.Size = new(1000, 1000);
    rectShape.FillColor = new Color(0, 0, 0, 100);
    window.Draw(rectShape);
#endif // DEBUG_OVERLAY
        }

        private void SetWindowIcon()
        {
            var img = new Image("C:\\Data\\MKWii\\icon.png");
            window.SetIcon(img.Size.X, img.Size.Y, img.Pixels);
        }
    }
}
