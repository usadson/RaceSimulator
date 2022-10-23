using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Pfim;
using ImageFormat = Pfim.ImageFormat;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Size = System.Drawing.Size;

namespace GUIApplication;

public static class SpriteManager
{
#if LOCKING_WORKS
    private static readonly object Lock = new();
#endif
    private static readonly Dictionary<string, Bitmap?> BitmapCache = new();
    public static readonly Dictionary<string, GCHandle> BitmapHandles = new();

    public static void SetEmptyBitmap(Size size, Bitmap bitmap)
    {
        var uri = $"@Empty_{size.Width}_{size.Height}";
        BitmapCache[uri] = bitmap;
    }

    public static Bitmap GetEmptyBitmap(int width, int height)
    {
        var uri = $"@Empty_{width}_{height}";
        if (BitmapCache.TryGetValue(uri, out var bitmapInCache))
        {
            Debug.Assert(bitmapInCache != null, nameof(bitmapInCache) + " != null");
            return (Bitmap)bitmapInCache.Clone();
        }

        var bitmap = new Bitmap(width, height);
        BitmapCache.Add(uri, bitmap);
        return (Bitmap)bitmap.Clone();
    }

    public static Bitmap? Load(string uri)
    {
        if (BitmapCache.TryGetValue(uri, out var bitmapInCache)) 
            return bitmapInCache;
        
        Bitmap? bitmap;
        try
        {
            bitmap = new(uri);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            bitmap = null;
        }

        BitmapCache.Add(uri, bitmap);
        return bitmap;
    }

    public static Bitmap? LoadTga(string fileName)
    {
        if (BitmapCache.TryGetValue(fileName, out var bitmapInCache))
            return bitmapInCache;

        var image = Pfimage.FromFile(fileName);

        PixelFormat? format = image.Format switch
        {
            ImageFormat.Rgb24 => PixelFormat.Format24bppRgb,
            ImageFormat.Rgba32 => PixelFormat.Format32bppArgb,
            ImageFormat.R5g5b5 => PixelFormat.Format16bppRgb555,
            ImageFormat.R5g6b5 => PixelFormat.Format16bppRgb565,
            ImageFormat.R5g5b5a1 => PixelFormat.Format16bppArgb1555,
            ImageFormat.Rgb8 => PixelFormat.Format8bppIndexed,
            _ => null
        };

        var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
        BitmapHandles.Add(fileName, handle);

        var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
        var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format!.Value, ptr);

        BitmapCache.Add(fileName, bitmap);
        return bitmap;
    }

    public static void ClearCache()
    {
        BitmapCache.Clear();
        BitmapHandles.Clear();
    }
}
