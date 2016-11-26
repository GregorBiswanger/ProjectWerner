using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWerner.SwitchApplication.Utilities
{
    public class KeyboardHelper
    {
        private const byte VK_MENU = 0x12;
        private const byte VK_TAB = 0x09;
        private const int KEYEVENTF_EXTENDEDKEY = 0x01;
        private const int KEYEVENTF_KEYUP = 0x02;

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static void AltTab()
        {
            keybd_event(VK_MENU, 0xb8, 0, 0); //Alt Press 
            keybd_event(VK_TAB, 0x8f, 0, 0); // Tab Press 
            keybd_event(VK_TAB, 0x8f, KEYEVENTF_KEYUP, 0); // Tab Release 
            keybd_event(VK_MENU, 0xb8, KEYEVENTF_KEYUP, 0); // Alt Release 
        }
    }
}
