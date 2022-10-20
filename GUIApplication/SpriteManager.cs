using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace GUIApplication;

public static class SpriteManager
{
#if LOCKING_WORKS
    private static readonly object Lock = new();
#endif
    private static readonly Dictionary<string, Bitmap?> BitmapCache = new();

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
        
        Debug.WriteLine($"Loading URI: \"{uri}\"");

        Bitmap? bitmap;
        try
        {
            bitmap = new(uri);
        }
        catch
        {
            bitmap = null;
        }

        BitmapCache.Add(uri, bitmap);
        return bitmap;
    }

    public static void ClearCache()
    {
        BitmapCache.Clear();
    }
}
