using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace RaceSimulator;

public static class ConsoleAPI
{
    [DllImport("kernel32.dll", EntryPoint = "GetConsoleWindow", SetLastError = true)]
    public static extern IntPtr GetConsoleHandle();

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(uint handle);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    public static extern bool GetCurrentConsoleFontEx(IntPtr consoleOutput, bool maximumWindow, ref ConsoleFontInfoEx info);

    [DllImport("user32.dll")]
    public static extern int GetWindowRect(IntPtr windowHandle, out Rectangle rect);

    private static uint StdOutputHandle = uint.MaxValue - 11; // (uint)-11;
    private static readonly IntPtr InvalidHandleValue = new(-1);

    public static Size GetConsoleSymbolSize()
    {
        var handle = GetStdHandle(StdOutputHandle);
        Debug.WriteLine($"Win32 Error (handle): {Marshal.GetLastWin32Error()}");
        Debug.Assert(!handle.Equals(InvalidHandleValue));
        
        ConsoleFontInfoEx info = new();
        info.StructureSize = (uint)Marshal.SizeOf(info);

        var result = GetCurrentConsoleFontEx(handle, false, ref info);

        Debug.WriteLine($"Win32 Error: {Marshal.GetLastWin32Error()}");
        Debug.Assert(result, Marshal.GetLastWin32Error().ToString());
        return new(info.FontSize.X, info.FontSize.Y);
    }

    public struct Coordinates
    {
        public short X { get; set; }
        public short Y { get; set; }
    }

    public const int LfFaceSize = 32;

    // CONSOLE_FONT_INFOEX 
    // https://learn.microsoft.com/en-us/windows/console/console-font-infoex
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct ConsoleFontInfoEx
    {
        public uint StructureSize;         
        public uint FontIndex;              
        public Coordinates FontSize;        
        public uint FontFamily;             
        public uint FontWeight;             

        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        //public string FontName;             
        public fixed char FontName[LfFaceSize];
    }
}
