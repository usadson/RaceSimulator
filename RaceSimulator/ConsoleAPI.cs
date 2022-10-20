using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RaceSimulator;

public static class ConsoleAPI
{
    [DllImport("kernel32.dll", EntryPoint = "GetConsoleWindow", SetLastError = true)]
    public static extern IntPtr GetConsoleHandle();

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetConsoleScreenBufferInfo(IntPtr consoleOutput, out ConsoleScreenBufferInfo info);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(uint handle);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern Coordinates GetConsoleFontSize(IntPtr consoleOutput, uint fontIndex);

    [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    public static extern bool GetCurrentConsoleFont(IntPtr consoleOutput, bool maximumWindow, ref ConsoleFontInfo info);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    public static extern bool GetCurrentConsoleFontEx(IntPtr consoleOutput, bool maximumWindow, ref ConsoleFontInfoEx info);

    [DllImport("user32.dll")]
    public static extern int GetWindowRect(IntPtr windowHandle, out Rectangle rect);

    [DllImport("user32.dll")]
    public static extern int UpdateWindow(IntPtr windowHandle);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref CallWinProcReturnStruct structure);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetWindowLongA(IntPtr windowHandle, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(IntPtr windowHandle, [Optional] out uint nIndex);

    private delegate int HookProc(int code, IntPtr wParam, ref CallWinProcReturnStruct lParam);

    [DllImport("user32.dll")]
    static extern IntPtr SetWindowsHookEx(int idHook, HookProc hookProc, IntPtr hMod, uint dwThreadId);

    public static uint StdOutputHandle = uint.MaxValue - 11; // (uint)-11;
    public static readonly IntPtr InvalidHandleValue = new(-1);

    private const int WM_PAINT = 15;

    private const int WH_GETMESSAGE = 3;
    private const int WH_CALLWNDPROC = 4;
    private const int WH_CALLWNDPROCRET = 12;

    private const int GWL_HINSTANCE = -6;

    private static IntPtr _hookHandle;

    private static uint _consoleProcess, _consoleThread;

    public static void Hook()
    {
        var hwnd = GetConsoleHandle();
        Debug.Assert(!hwnd.Equals(InvalidHandleValue));
        Debug.Assert(!hwnd.Equals(IntPtr.Zero));
        
        _consoleThread = GetWindowThreadProcessId(hwnd, out _consoleProcess);

        var ourProcess = Process.GetCurrentProcess().Id;
        Debug.Assert(_consoleProcess != ourProcess);

        var consoleInstance = GetWindowLongA(hwnd, GWL_HINSTANCE);
        
        var instance = Marshal.GetHINSTANCE(typeof(Program).Module);
        Debug.Assert(!instance.Equals(new IntPtr(-1)));
        
        Debug.Assert(!instance.Equals(InvalidHandleValue));

        _hookHandle = SetWindowsHookEx(WH_GETMESSAGE, GetMessageHook, instance, 0);
        Debug.Assert(!_hookHandle.Equals(InvalidHandleValue));
    }

    private static int GetMessageHook(int code, IntPtr wParam, ref CallWinProcReturnStruct structure)
    {
        uint processId;
        uint threadId = GetWindowThreadProcessId(structure.WindowHandle, out processId);
        if (processId == _consoleProcess)
        {
            Debug.Assert(threadId == _consoleThread);
            Debug.WriteLine("YESSS");
        }

        return CallNextHookEx(_hookHandle, code, wParam, ref structure);
    }

    public static Size GetConsoleSymbolSize()
    {
        var handle = GetStdHandle(StdOutputHandle);
        Debug.WriteLine($"Win32 Error (handle): {Marshal.GetLastWin32Error()}");
        Debug.Assert(!handle.Equals(InvalidHandleValue));

#if no
        ConsoleFontInfo info;
        var result = GetCurrentConsoleFont(handle, false, out info);
#endif
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

    public struct SmallRect
    {
        public short Left { get; set; }
        public short Top { get; set; }
        public short Right { get; set; }
        public short Bottom { get; set; }
    }

    // https://learn.microsoft.com/en-us/windows/console/console-screen-buffer-info-str
    public struct ConsoleScreenBufferInfo
    {
        public Coordinates Size { get; set; }
        public Coordinates CursorPosition { get; set; }
        public short Attributes { get; set; }
        public SmallRect ConsoleScreenBufferRect { get; set; }
        public Coordinates MaximumWindowSize { get; set; }
    }

    // https://learn.microsoft.com/en-us/windows/console/console-font-info-str
    public struct ConsoleFontInfo
    {
        public uint FontIndex;
        public Coordinates FontSize;
    }

    public const int LF_FACESIZE = 32;

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
        public fixed char FontName[LF_FACESIZE];
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CallWinProcReturnStruct
    {
        public IntPtr Result;
        public IntPtr LParam;
        public IntPtr WParam;
        public uint Message;
        public IntPtr WindowHandle;
    }
}
