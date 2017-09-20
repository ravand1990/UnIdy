using System.Windows.Forms;
using PoeHUD.Hud.Settings;
using PoeHUD.Plugins;

namespace UnIdy
{
    internal class Settings : SettingsBase
    {
        public Settings()
        {
            Enable = true;
            HotKey = Keys.F2;
            Speed = new RangeNode<int>(20, 0, 100);
            Identification = true;
            Magic = true;
            Rare = true;
            Unique = true;
            Map = false;
            Debug = false;
        }

        [Menu("Hotkey")]
        public HotkeyNode HotKey { get; set; }

        [Menu("Speed")]
        public RangeNode<int> Speed { get; set; }

        [Menu("Identify What?", 1000)]
        public ToggleNode Identification { get; set; }

        [Menu("Magic", 1001, 1000)]
        public ToggleNode Magic { get; set; }

        [Menu("Rare", 1002, 1000)]
        public ToggleNode Rare { get; set; }

        [Menu("Unique", 1003, 1000)]
        public ToggleNode Unique { get; set; }

        [Menu("Map", 1004, 1000)]
        public ToggleNode Map { get; set; }

        [Menu("Debug")]
        public ToggleNode Debug { get; set; }
    }
}