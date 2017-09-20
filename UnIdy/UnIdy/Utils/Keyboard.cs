using System.Runtime.InteropServices;

namespace UnIdy.Utils
{
    internal class Keyboard
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);


        public static bool hold = false;

        public static void HoldKey(byte key)
        {
            keybd_event(key, 0, (int) KeyboardEvents.KEY_DOWN, 0);
        }

        public static void ReleaseKey(byte key)
        {
            keybd_event(key, 0, (int) KeyboardEvents.KEY_DOWN | (int) KeyboardEvents.KEY_UP, 0);
        }

        public enum KeyboardEvents
        {
            KEY_DOWN = 0x0001,
            KEY_UP = 0x0002
        }
    }
}