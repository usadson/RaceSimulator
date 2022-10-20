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

namespace GUIApplication
{
    public sealed class Renderer : AbstractTrackRenderer, IDisposable
    {
        private enum RenderMode
        {
            Straights,
            Corners,
            Decor,


            CurbCorners,

            Participants,
        }

        private readonly Race _race;
        public Size FrameSize { get; set; }

        private Bitmap? _bitmap;
        private Graphics? _graphics;
        private readonly TexturePack _texturePack = new();

        private readonly float _initialScaling;
        private float _fitToFrameScaling = 1.0f;
        private float Scaling => _initialScaling * _fitToFrameScaling;
        private RenderMode _renderMode;

        public Renderer(Race race, Size windowSize, float scaling = 1.0f)
        {
            FrameSize = windowSize;
            _initialScaling = scaling;
            _race = race;
            CalculateTrackSize();
            Debug.WriteLine("Width: {0}, Height: {0}",
                BottomRightmostPoint.X - TopLeftmostPoint.X,
                BottomRightmostPoint.Y - TopLeftmostPoint.Y);
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
            CalculateScaling();

            _bitmap = Draw(FrameSize.Width, FrameSize.Height);
            return CreateBitmapSourceFromGdiBitmap(_bitmap);
        }

        public Bitmap Draw(int width, int height)
        {
            var bitmap = SpriteManager.GetEmptyBitmap(width, height);
            _bitmap = bitmap;
            
            _graphics = Graphics.FromImage(bitmap);

            _graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            foreach (var renderMode in Enum.GetValues<RenderMode>())
            {
                X = -TopLeftmostPoint.X * Scaling;
                Y = -TopLeftmostPoint.Y * Scaling;

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

            if (section.SectionType == SectionTypes.RightCorner)
            {
                Debug.WriteLine($"RightCorner dir={direction}");
            }

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

            }
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

            var points = new PointF[]
            {
                point,
                new(point.X + width, point.Y),
                new(point.X, point.Y + height)
            };

            _graphics.DrawImage(bitmap, points);
        }

        public void Dispose()
        {
            _bitmap?.Dispose();
            _graphics?.Dispose();
        }
    }
}
