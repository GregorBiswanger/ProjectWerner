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
        private const byte VkMenu = 0x12;
        private const byte VkTab = 0x09;
        private const int KeyeventfExtendedkey = 0x01;
        private const int KeyeventfKeyup = 0x02;

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static void AltTab()
        {
            keybd_event(VkMenu, 0xb8, 0, 0); //Alt Press 
            keybd_event(VkTab, 0x8f, 0, 0); // Tab Press 
            keybd_event(VkTab, 0x8f, KeyeventfKeyup, 0); // Tab Release 
            keybd_event(VkMenu, 0xb8, KeyeventfKeyup, 0); // Alt Release 
        }
    }
}
