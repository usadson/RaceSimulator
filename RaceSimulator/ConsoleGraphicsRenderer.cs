using System.Diagnostics;
using System.Drawing;
using System.Runtime.Versioning;
using Controller;
using Model;

namespace RaceSimulator;

[SupportedOSPlatform("windows")]
internal class ConsoleGraphicsRenderer : IDisposable
{
    private Image? _bitmap;
    private readonly IntPtr _handle;

    private Rectangle _rect;
    private Size _characterSize;

    private float IconWidth => _characterSize.Width;
    private float IconHeight => _characterSize.Height;

    public delegate void ReadyEventHandler();
    public event ReadyEventHandler? OnReady;
    private Graphics gfx;

    public bool IsReady
    {
        get
        {
            if (_bitmap is null)
                return false;

            if (_characterSize.Width == 0 || _characterSize.Height == 0)
                return false;
            
            if (_handle.Equals(IntPtr.Zero))
                return false;

            return true;
        }
    }

    public ConsoleGraphicsRenderer()
    {
        //Task.Run(() => _bitmap = Image.FromFile("C:\\Data\\MKWii\\minimap_icons.png"))
        //    .ContinueWith(_ => OnReady?.Invoke());

        OnReady += OnResize;

        _handle = ConsoleAPI.GetConsoleHandle();
        gfx = Graphics.FromHwnd(_handle);
        OnResize();
    }

    public void OnResize()
    {
        ConsoleAPI.GetWindowRect(_handle, out _rect);

        _characterSize = ConsoleAPI.GetConsoleSymbolSize();
        Debug.Assert(_characterSize.Width != 0);
        Debug.Assert(_characterSize.Height != 0);
    }

    private SizeF CreateSize(SizeF size)
    {
        float aspectRatio = size.Width / size.Height;
        return new(
            _characterSize.Width,
            _characterSize.Height * aspectRatio
        );
    }

    public void DrawIcon(Character character, PointF point)
    {
        if (!IsReady)
            return;

        // using (var bmp = new Bitmap(600, 400))
        // using (var gfx = Graphics.FromImage(bmp))
        gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var offset = new RectangleF(0, 0, 10.0f, 10.0f);
        if (SpriteOffsets.MinimapOffsets.TryGetValue(character, out var offsets))
        {
            offset = new(offsets.Left, offsets.Top, offsets.Width, offsets.Height);
        }

        gfx.DrawImage(_bitmap, new RectangleF(point, CreateSize(offset.Size)), offset, GraphicsUnit.Pixel);

        //UpdateWindow(_handle);
    }

    public void DrawSection(ConsoleSymbol symbol, SectionData sectionData)
    {
        if (sectionData.Left.Participant is null && sectionData.Right.Participant is null)
            return;

        if (sectionData.Left.Participant is not null)
        {
            DrawIcon((sectionData.Left.Participant as Driver).Character, TranslatePositionToPoint(symbol.LeftPosition));
        }

        if (sectionData.Right.Participant is not null)
        {
            DrawIcon((sectionData.Right.Participant as Driver).Character, TranslatePositionToPoint(symbol.RightPosition));
        }
    }

    private PointF TranslatePositionToPoint((int X, int Y) position)
    {
        Debug.Assert(_characterSize.Width != 0);
        Debug.Assert(_characterSize.Height != 0);
        return new PointF(
            (Console.CursorLeft + position.X) * (float)_characterSize.Width,
            (Console.CursorTop + position.Y) * (float)_characterSize.Height
        );
    }

    public void Dispose()
    {
        OnReady = null;
        gfx.Dispose();
    }
}
