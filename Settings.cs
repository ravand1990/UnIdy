using System.Windows.Forms;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace UnIdy
{
    public class Settings : ISettings
    {
        public Settings()
        {
            Enable = new ToggleNode(true);
            HotKey = new HotkeyNode(Keys.F2);
            ExtraDelay = new RangeNode<int>(20, 0, 100);
            Identification = new ToggleNode(true);
            IdentifyMagicItems = new ToggleNode(true);
            IdentifyRares = new ToggleNode(true);
            IdentifyUniques = new ToggleNode(true);
            IdentifyMaps = new ToggleNode(false);
            IdentifyItemsWithRedGreenBlueLinks = new ToggleNode(true);
            IdentifySixSockets = new ToggleNode(true);
            IdentifyVisibleTabItems = new ToggleNode(true);
            Debug = new ToggleNode(true);
        }

        [Menu("Enable")]
        public ToggleNode Enable { get; set; }

        [Menu("Hotkey")]
        public HotkeyNode HotKey { get; set; }

        [Menu("Identify visible stashtab items")]
        public ToggleNode IdentifyVisibleTabItems { get; set; }

        [Menu("Extra Delay", "Additional delay, plugin should work without extra delay, this is merely optional.")]
        public RangeNode<int> ExtraDelay { get; set; }

        [Menu("Identify What?", 1000)]
        public ToggleNode Identification { get; set; }

        [Menu("Magic items", 1001, 1000)]
        public ToggleNode IdentifyMagicItems { get; set; }

        [Menu("Rares", 1002, 1000)]
        public ToggleNode IdentifyRares { get; set; }

        [Menu("Uniques", 1003, 1000)]
        public ToggleNode IdentifyUniques { get; set; }

        [Menu("6-sockets", 1004, 1000)]
        public ToggleNode IdentifySixSockets { get; set; }

        [Menu("RGB (chromatic)", 1005, 1000)]
        public ToggleNode IdentifyItemsWithRedGreenBlueLinks { get; set; }

        [Menu("Map", 1006, 1000)]
        public ToggleNode IdentifyMaps { get; set; }

        [Menu("Debug")]
        public ToggleNode Debug { get; set; }
    }
}