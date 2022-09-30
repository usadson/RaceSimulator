using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RaceSimulator
{
    internal class ConsoleWindowManager
    {
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
        IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
           hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
           uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        const uint EVENT_OBJECT_NAMECHANGE = 0x800C;
        const uint EVENT_OBJECT_IME_CHANGE = 0x8029;
        const uint WINEVENT_OUTOFCONTEXT = 0;

        // Need to ensure delegate is not collected while we're using it,
        // storing it in a class field is simplest way to do this.
        static WinEventDelegate procDelegate = new(WinEventProc);

        private static IntPtr _handle;
        private static IntPtr _hook;

        public static void Initialize()
        {
            new Thread(() =>
            {
                _handle = Process.GetCurrentProcess().MainWindowHandle;
                Console.WriteLine($"Handle: {9:x8}", _handle.ToInt64());
                _hook = SetWinEventHook(0x00000001, 0x7FFFFFFF, IntPtr.Zero,
                    procDelegate, (uint)Environment.ProcessId, 0, 0);
                Console.WriteLine($"Hook: {_hook}");
            }).Start();
        }

        public static void Deinitialize()
        {
            UnhookWinEvent(_hook);
        }

        private static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, 
            int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            Console.WriteLine($"{0:x8} event {eventType}", hwnd.ToInt32());
        }
    }
}
