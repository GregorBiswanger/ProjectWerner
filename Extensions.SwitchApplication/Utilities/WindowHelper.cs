using System;
using System.Runtime.InteropServices;

namespace ProjectWerner.SwitchApplication.Utilities
{
    public class WindowHelper
    {
        public const int SwHide = 0;
        public const int SwNormal = 1;
        public const int SwShowminimized = 2;
        public const int SwShowmaximized = 3;
        public const int SwShownoactivate = 4;
        public const int SwShow = 5;
        public const int SwMinimize = 6;
        public const int SwShowminnoactive = 7;
        public const int SwShowna = 8;
        public const int SwRestore = 9;
        public const int SwShowdefault = 10;

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }

}
