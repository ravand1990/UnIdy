using System.Collections.Generic;
using PoeHUD.Plugins;
using PoeHUD.Hud.Settings;
using System.Windows.Forms;
namespace UnIdy
{
    class Settings:SettingsBase
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
        }
        [Menu("Hotkey")]
        public HotkeyNode HotKey { get; set; }
        [Menu("Speed")]
        public RangeNode<int> Speed { get; set; }

        [Menu("Identify What?",1000)]
        public ToggleNode Identification { get; set; }

        [Menu("Magic", 1001,1000)]
        public ToggleNode Magic { get; set; }
        [Menu("Rare", 1002,1000)]
        public ToggleNode Rare { get; set; }
        [Menu("Unique", 1003,1000)]
        public ToggleNode Unique { get; set; }

    }
}
