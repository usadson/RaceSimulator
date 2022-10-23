using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonViewLib;
using Controller;
using Model;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;

namespace GUIApplication
{
    public sealed class Renderer : AbstractTrackRenderer, IDisposable
    {
        public static readonly object RenderLock = new();

        private enum RenderMode
        {
            Straights,
            Corners,
            Decor,


            CurbCorners,

            Participants,
        }
        
        public Size FrameSize { get; set; }

        private Bitmap? _bitmap;
        private Graphics? _graphics;
        private readonly TexturePack _texturePack = new();

        private readonly float _initialScaling;
        private float _fitToFrameScaling = 1.0f;
        private float Scaling => _initialScaling * _fitToFrameScaling;
        private RenderMode _renderMode;
        private bool _hasDrawnTrack;
        private readonly Font _font;
        private const string FontName = "Segoe UI";

        private uint _animationCounter, _fastAnimationCounter;
        private readonly System.Timers.Timer _animationTimer;

        public Renderer(Size windowSize, float scaling = 1.0f)
        {
            Debug.Assert(windowSize.Width != 0);
            Debug.Assert(windowSize.Height != 0);

            _font = new Font(FontName, 10);

            FrameSize = windowSize;
            _initialScaling = scaling;
                //_race = race;
            CalculateTrackSize();
            Debug.WriteLine("Width: {0}, Height: {0}",
                BottomRightmostPoint.X - TopLeftmostPoint.X,
                BottomRightmostPoint.Y - TopLeftmostPoint.Y);
            
            CalculateScaling();
            SpriteManager.SetEmptyBitmap(FrameSize, Draw(FrameSize.Width, FrameSize.Height));

            _animationTimer = new();
            _animationTimer.Interval = 10;
            _animationTimer.Elapsed += (_, _) =>
            {
                ++_fastAnimationCounter;
                if (_fastAnimationCounter % 20 == 0)
                    ++_animationCounter;
            };
            _animationTimer.Start();
        }

        private void CalculateScaling()
        {
            var size = new SizeF(BottomRightmostPoint.X - TopLeftmostPoint.X, BottomRightmostPoint.Y - TopLeftmostPoint.Y);
            Debug.Assert(size.Width != 0);
            Debug.Assert(size.Height != 0);

            Debug.Assert(FrameSize.Width != 0);
            Debug.Assert(FrameSize.Height != 0);
            _fitToFrameScaling = Math.Min(FrameSize.Width / size.Width * 0.8f, FrameSize.Height / size.Height * 0.8f);
            Debug.Assert(_fitToFrameScaling > 0);
        }

        private void CalculateTrackSize()
        {
            _fitToFrameScaling = 1 / _initialScaling;
            _graphics = null;
            StartDrawTrack();
            _fitToFrameScaling = 1;
        }

        public BitmapSource Draw()
        {
            lock (RenderLock)
            {
                CalculateScaling();

                _bitmap = Draw(FrameSize.Width, FrameSize.Height);
                return CreateBitmapSourceFromGdiBitmap(_bitmap);
            }
        }

        private Bitmap Draw(int width, int height)
        {
            var hasDrawnTrack = _hasDrawnTrack;

            var bitmap = SpriteManager.GetEmptyBitmap(width, height);
            _bitmap = bitmap;
            
            _graphics = Graphics.FromImage(bitmap);

            _graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            
            foreach (var renderMode in Enum.GetValues<RenderMode>())
            {
                if (hasDrawnTrack)
                {
                    if (renderMode != RenderMode.Participants)
                        continue;
                }
                else
                {
                    if (renderMode == RenderMode.Participants)
                    {
                        _hasDrawnTrack = true;
                        break;
                    }
                }

                X = -TopLeftmostPoint.X * Scaling + 50;
                Y = -TopLeftmostPoint.Y * Scaling + 50;
                _renderMode = renderMode;
                StartDrawTrack();
            }
            
            _graphics.Dispose();
            _graphics = null;

            _bitmap = null;
            return bitmap;
        }

        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
        {
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        private Bitmap GetBitmapBySectionType(SectionTypes sectionType, Direction direction, string layer = "Main")
        {
            return _texturePack.Load(sectionType, direction, layer);
        }

        protected override SizeF GetSizeOfSection(SectionTypes sectionType, Direction direction)
        {
            var size = GetBitmapBySectionType(sectionType, direction).Size;
            return new SizeF(size.Width * Scaling, size.Height * Scaling);
        }

        protected override void DirectionChanged(Direction oldDirection, Direction newDirection, SectionTypes section)
        {
            var offset = _texturePack.GetOffsetDirectionChange(section, newDirection);
            X += offset.X * Scaling;
            Y += offset.Y * Scaling;
        }

        protected override void DrawSection(Direction direction, Section section, SectionData sectionData)
        {
            if (_graphics == null || _bitmap == null)
                return;

            var bitmap = GetBitmapBySectionType(section.SectionType, direction);

            var visualSize = new SizeF(bitmap.Width * Scaling, bitmap.Height * Scaling);
            var imageRect = _texturePack.GetBitmapSubsection(section.SectionType, direction, bitmap);

            var offset = new PointF(0, 0);
            //var offset = _texturePack.GetOffsetDirectionChange(section.SectionType, direction));
            var x = CurrentPoint.X + offset.X * Scaling;
            var y = CurrentPoint.Y + offset.Y * Scaling;

            var points = new PointF[]
            {
                // Upper-Left
                new(x, y),
                // Upper-Right
                new(x + visualSize.Width, y),
                // Lower-Left
                new(x, y + visualSize.Height)
            };
            
            switch (_texturePack.DirectionToDegrees(section.SectionType, direction))
            {
                case 0:
                    break;
                case 90:
                    // Upper-Left
                    points[0] = new(x + visualSize.Height, y + visualSize.Height);
                    // Upper-Right
                    points[1] = new(x, y + visualSize.Height);
                    // Lower-Left
                    points[1] = new(x + visualSize.Height, y);
                    break;
                case 180:
                    // Upper-Left
                    points[0] = new(x, y + visualSize.Height);
                    // Upper-Right
                    points[1] = new(x + visualSize.Width, y + visualSize.Height);
                    // Lower-Left
                    points[2] = new(x, y);
                    break;
                case 270:
                    /*// Upper-Left
                    points[0] = new(x, y + visualSize.Height);
                    // Upper-Right
                    points[1] = new(x + visualSize.Height, y + visualSize.Height);
                    // Lower-Left
                    points[1] = new(x, y);*/
                    break;
            }

            bool shouldDraw = false;
            switch (section.SectionType)
            {
                case SectionTypes.LeftCorner:
                case SectionTypes.RightCorner:
                    shouldDraw = _renderMode == RenderMode.Corners;
                    break;
                case SectionTypes.StartGrid:
                case SectionTypes.Finish:
                case SectionTypes.Straight:
                    shouldDraw = _renderMode == RenderMode.Straights;
                    break;
            }

            if (shouldDraw)
                _graphics.DrawImage(bitmap, points, imageRect, GraphicsUnit.Pixel);

            // Decor
            if (_renderMode == RenderMode.CurbCorners && section.SectionType == SectionTypes.RightCorner)
            {
                var roundCurb = SpriteManager.Load(@".\Assets\RoadCircle.png");
                if (roundCurb == null)
                    return;

                var width = roundCurb.Width * Scaling;
                var height = roundCurb.Height * Scaling;

                float beginX = x;
                float beginY = y;
                float curbOffset = 43 * Scaling;

                switch (direction)
                {
                    case Direction.North:
                        beginX += visualSize.Width - curbOffset;
                        beginY += visualSize.Height - curbOffset;
                        break;
                    case Direction.East:
                        beginX -= curbOffset;
                        beginY += visualSize.Height - curbOffset;
                        break;
                    case Direction.South:
                        beginX -= curbOffset;
                        beginY -= curbOffset;
                        break;
                    case Direction.West:
                        beginX += visualSize.Width - curbOffset;
                        beginY -= curbOffset;
                        break;
                }

                points = new PointF[]
                {
                    new(beginX, beginY),
                    new(beginX + width, beginY),
                    new(beginX, beginY + height)
                };

                _graphics.DrawImage(roundCurb, points);
            }

            if (_renderMode == RenderMode.Corners)
            {
                DrawCorners(section.SectionType, new PointF(x, y), visualSize, direction);
            }

            if (_renderMode == RenderMode.Decor)
            {
                switch (section.SectionType)
                {
                    case SectionTypes.Finish:
                    {
                        var finishLine = _texturePack.LoadFinish(direction);
                        var scaleX = (float)visualSize.Width / finishLine.Width;
                        var scaleY = (float)visualSize.Height / finishLine.Height;
                        var width = finishLine.Width * scaleX;
                        var height = finishLine.Height * scaleY;

                        float beginX = x + (visualSize.Width - width) / 2;
                        float beginY = y + (visualSize.Height - height) / 2;
                        points = new PointF[]
                        {
                            new(beginX, beginY),
                            new(beginX + width, beginY),
                            new(beginX, beginY + height)
                        };
                        _graphics.DrawImage(finishLine, points, imageRect, GraphicsUnit.Pixel);
                        break;
                    }
                }
            }

            if (_renderMode == RenderMode.Participants)
            {
                //1 / (X + Y) < _random.NextDouble()
                if (CurrentSectionIndex % 4 == 0)
                {
                    DrawDecorObject(section, direction, new PointF(x, y), visualSize);
                }

                if (sectionData.Left.Participant == null)
                    return;
                var bitmapIcons = SpriteManager.Load(@".\Assets\character_icons.png");
                
                if (sectionData.Left.Participant != null)
                    DrawParticipant(bitmapIcons, section.SectionType, sectionData.Left, direction, false, new PointF(x, y), visualSize);

                if (sectionData.Right.Participant != null)
                    DrawParticipant(bitmapIcons, section.SectionType, sectionData.Right, direction, true, new PointF(x, y), visualSize);
            }
        }

        private void DrawDecorObject(Section section, Direction direction, PointF pointF, SizeF visualSize)
        {
            Debug.Assert(_graphics != null);

            var random = new Random((int)(X + Y));
            bool animation = DateTime.Now.Millisecond % 500 < 250;

            var xFactor = 0.0f;
            var yFactor = 0.0f;
            switch (direction)
            {
                case Direction.North:
                    xFactor = -1.1f;
                    break;
                case Direction.East:
                    yFactor = -1.1f;
                    break;
                case Direction.South:
                    xFactor = 0.2f;
                    pointF = pointF with { X = X + visualSize.Width };
                    break;
                case Direction.West:
                    xFactor = 0.2f;
                    pointF = pointF with { Y = Y + visualSize.Height };
                    break;
            }

            Bitmap? bitmap;
            RectangleF srcRect;
            SizeF renderSize;

            switch (random.Next(0, 4))
            {
                case 0:
                case 1:
                    bitmap = SpriteManager.Load(@"Assets\Plant.png");
                    srcRect = new(animation ? 75 : 11, 7, 62, 57);
                    renderSize = new SizeF(
                        srcRect.Width * 0.55f,
                        srcRect.Height * 0.55f
                    );
                    break;
                case 2:
                    bitmap = SpriteManager.Load(@"Assets\Crab.png");
                    srcRect = new(0, animation ? 32 : 0, 37, 32);
                    renderSize = new SizeF(34.1f, 31.35f);
                    break;
                case 3:
                    var animationFrame = _animationCounter % 12;
                    bitmap = SpriteManager.Load(@"Assets\Goomba.png");
                    Debug.Assert(bitmap != null);
                    srcRect = new(animationFrame * 64, 0, 64, 64);
                    renderSize = new SizeF(34.1f, 31.35f);
                    break;
                default:
                    return;
            }

            if (bitmap == null)
                return;

            pointF = new(
                pointF.X + renderSize.Width * xFactor,
                pointF.Y + renderSize.Height * yFactor
            );

            PointF[] points = {
                pointF,
                pointF with { X = pointF.X + renderSize.Width },
                pointF with { Y = pointF.Y + renderSize.Height }
            };

            if (pointF.Y <= 0)
                return;

            _graphics.DrawImage(bitmap, points, srcRect, GraphicsUnit.Pixel);
        }

        private void DrawParticipant(Bitmap bitmapIcons, SectionTypes sectionType, SectionData.Lane lane, Direction direction, bool isRightLane, PointF point, SizeF visualSize)
        {
            Debug.Assert(lane.Participant != null);
            Debug.Assert(_graphics != null);

            float size = Math.Min(visualSize.Width, visualSize.Height) / 3 * 2;
            float percentage = (float)lane.Distance / SectionRegistry.Lengths[sectionType];
            if (direction is Direction.East or Direction.West)
            {
                point = point with
                {
                    X = X + percentage * visualSize.Width * (direction is Direction.East ? 1 : -1),
                    Y = Y + (isRightLane ? size : 0)
                };
            }
            else
            {
                point = point with
                {
                    X = X + (isRightLane ? visualSize.Width - size : 0),
                    Y = Y + percentage * visualSize.Height * (direction is Direction.South ? 1 : -1)
                    /*X = X + (visualSize.Width - size) / 2,
                    Y = Y + visualSize.Width / 3 * (isRightLane ? 1 : 2)*/
                };
            }

            var points = new[]
            {
                point,
                point with { X = point.X + size },
                point with { Y = point.Y + size }
            };

            var character = ((Driver)lane.Participant).Character;
            var offsets = SpriteOffsets.MinimapOffsets[character];
            RectangleF srcRect = new(offsets.Left, offsets.Top, offsets.Width, offsets.Height);

            _graphics.DrawImage(bitmapIcons, points, srcRect, GraphicsUnit.Pixel);

            if (lane.Participant.Equipment.IsBroken)
            {
                uint index = _fastAnimationCounter % 100 + 1;
                string indexStr = index.ToString("D3");
                Debug.Assert(indexStr.Length == 3);
                string fileName = @$"Assets\Particles\explosion01-frame{indexStr}.tga";
                var bitmap = SpriteManager.LoadTga(fileName);
                Debug.Assert(bitmap != null);

                float width = bitmap.Width * 0.3f;
                float height = bitmap.Height * 0.3f;
                points[0] = point with { Y = point.Y - height / 2 };
                points[1] = point with { X = point.X + width, Y = point.Y - height / 2};
                points[2] = point with { Y = point.Y + height / 2 };
                _graphics.DrawImage(bitmap, points);
            }

            var characterName = I18N.Translate(character.ToString());
            var textSize = _graphics.MeasureString(characterName, _font);
            var stringPoint = new PointF(
                point.X + (size - textSize.Width) / 2,
                point.Y + size + 2
            );
            var rectangle = new RectangleF(stringPoint.X - 2, stringPoint.Y - 2, textSize.Width + 2,
                textSize.Height + 2);
            _graphics.FillRectangle(new SolidBrush(Color.FromArgb(90, 0, 0, 0)), rectangle);
            _graphics.DrawString(characterName, _font, new SolidBrush(Color.White), stringPoint);
        }

        private void DrawCorners(SectionTypes sectionType, PointF point, SizeF sectionSize, Direction direction)
        {
            switch (sectionType)
            {
                case SectionTypes.StartGrid:
                case SectionTypes.Finish:
                case SectionTypes.Straight:
                    var left = "LeftCurb";
                    var right = "RightCurb";

                    if (direction is Direction.East or Direction.West)
                    {
                        left = "BottomCurb";
                        right = "TopCurb";
                    }

                    DrawCorner(_texturePack.Load(sectionType, direction, left),
                        sectionType, point, sectionSize, direction);

                    DrawCorner(_texturePack.Load(sectionType, Directions.Opposite(direction), right),
                        sectionType, point, sectionSize, direction);

                    break;
                default:
                    break;
            }
        }

        private void DrawCorner(Bitmap bitmap, SectionTypes sectionType, PointF point, SizeF sectionSize, Direction direction)
        {
            var width = bitmap.Width * Scaling;
            var height = bitmap.Height * Scaling;
            
            float curbOffset = 43 * Scaling;

            point = point with
            {
                X = X - curbOffset,
                Y = Y - curbOffset
            };

            switch (direction)
            {
                case Direction.North:
                    point = point with
                    {
                        X = X + sectionSize.Width,
                        Y = Y + sectionSize.Height
                    };
                    break;
                case Direction.East:
                    point = point with
                    {
                        Y = Y + sectionSize.Height
                    };
                    break;
                case Direction.South:
                    break;
                case Direction.West:
                    point = point with
                    {
                        X = X + sectionSize.Width
                    };
                    break;
            }

            var points = new[]
            {
                point,
                point with { X = point.X + width },
                new(point.X, point.Y + height)
            };

            _graphics.DrawImage(bitmap, points);
        }

        public void Dispose()
        {
            _bitmap?.Dispose();
            _graphics?.Dispose();
            _animationTimer.Dispose();
        }
    }
}
