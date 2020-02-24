using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using SharpDX;

namespace UnIdy
{
    public class UnIdy : BaseSettingsPlugin<Settings>
    {
        private IngameState _ingameState;
        private Vector2 _windowOffset;
        private Coroutine CoroutineWorker;
        private const string coroutineName = "UnIdy";

        public UnIdy()
        {
        }

        public override bool Initialise()
        {
            base.Initialise();
            Name = "UnIdy";

            _ingameState = GameController.Game.IngameState;
            _windowOffset = GameController.Window.GetWindowRectangle().TopLeft;

            Input.RegisterKey(Settings.HotKey.Value);
            Input.RegisterKey(Keys.LShiftKey);

            Settings.HotKey.OnValueChanged += () => { Input.RegisterKey(Settings.HotKey.Value); };

            return true;
        }

        public override void Render()
        {
            base.Render();

            var inventoryPanel = _ingameState.IngameUi.InventoryPanel;

            if (!inventoryPanel.IsVisible)
                return;

            if (Settings.HotKey.PressedOnce())
            {
                CoroutineWorker = new Coroutine(Identify(), this, coroutineName);
                Core.ParallelRunner.Run(CoroutineWorker);
            }
        }

        /*
        private void DrawPluginImageAndText()
        {
            var inventoryPanel = _ingameState.IngameUi.InventoryPanel;
            var playerInventory = inventoryPanel[InventoryIndex.PlayerInventory];
            var pos = playerInventory.InventoryUiElement.GetClientRect().TopLeft;
            pos.Y -= 100;
            const int height = 35;
            const int width = 169;
            var rec = new RectangleF(pos.X, pos.Y, width, height);
            pos.Y += height;
            Graphics.DrawPluginImage($"{PluginDirectory}//img//logo.png", rec);
            Graphics.DrawText($"Is running\nPress {Settings.HotKey.Value} to stop.", 20, pos);
        }
        */

        private IEnumerator Identify()
        {
            var inventoryPanel = _ingameState.IngameUi.InventoryPanel;
            var playerInventory = inventoryPanel[InventoryIndex.PlayerInventory];

            var scrollOfWisdom = GetItemWithBaseName("Scroll of Wisdom", playerInventory.VisibleInventoryItems);
            LogMessage(scrollOfWisdom.Text, 1);

            if (scrollOfWisdom == null)
                yield break;

            var normalInventoryItems = playerInventory.VisibleInventoryItems;

            if (Settings.IdentifyVisibleTabItems.Value && _ingameState.IngameUi.StashElement.IsVisible)
            {
                foreach (var normalStashItem in _ingameState.IngameUi.StashElement.VisibleStash.VisibleInventoryItems)
                {
                    normalInventoryItems.Insert(normalInventoryItems.Count,normalStashItem);
                }
            }

            var latency = (int)_ingameState.CurLatency;
            var listOfNormalInventoryItemsToIdentify = new List<NormalInventoryItem>();

            foreach (var normalInventoryItem in normalInventoryItems)
            {
                if (normalInventoryItem.Item.HasComponent<Mods>())
                {
                    var mods = normalInventoryItem.Item.GetComponent<Mods>();

                    if (mods.Identified)
                    {
                        continue;
                    }

                    switch (mods.ItemRarity)
                    {
                        case ItemRarity.Unique when !Settings.IdentifyUniques.Value:
                            continue;
                        case ItemRarity.Rare when !Settings.IdentifyRares.Value:
                            continue;
                        case ItemRarity.Magic when !Settings.IdentifyMagicItems.Value:
                            continue;
                        case ItemRarity.Normal:
                            continue;
                        default:
                            break;
                    }

                    var sockets = normalInventoryItem.Item.GetComponent<Sockets>();
                    if (!Settings.IdentifySixSockets.Value && sockets.NumberOfSockets == 6)
                    {
                        continue;
                    }

                    if (!Settings.IdentifyItemsWithRedGreenBlueLinks.Value && sockets.IsRGB)
                    {
                        continue;
                    }

                    var itemIsMap = normalInventoryItem.Item.HasComponent<Map>();
                    if (!Settings.IdentifyMaps.Value && itemIsMap)
                    {
                        continue;
                    }

                    listOfNormalInventoryItemsToIdentify.Add(normalInventoryItem);

                }
            }

            if (listOfNormalInventoryItemsToIdentify.Count == 0)
                yield break;

            #region Mouse click

            yield return Input.SetCursorPositionSmooth(scrollOfWisdom.GetClientRect().Center + _windowOffset);

            yield return new WaitTime(Settings.ExtraDelay.Value / 2);

            Input.Click(MouseButtons.Right);

            #endregion

            yield return new WaitTime(latency);

            Input.KeyDown(Keys.LShiftKey);
            foreach (var normalInventoryItem in listOfNormalInventoryItemsToIdentify)
            {
                if (Settings.Debug.Value)
                {
                    //Graphics.DrawFrame(normalInventoryItem.GetClientRect(), 2, Color.AliceBlue);
                }

                #region Mouse click

                yield return Input.SetCursorPositionSmooth(normalInventoryItem.GetClientRect().Center + _windowOffset);

                yield return new WaitTime(Settings.ExtraDelay.Value / 2);

                Input.Click(MouseButtons.Left);

                yield return new WaitTime(Settings.ExtraDelay.Value);

                #endregion
            }
            Input.KeyUp(Keys.LShiftKey);

            yield break;
        }

        private NormalInventoryItem GetItemWithBaseName(string baseName,
            IEnumerable<NormalInventoryItem> normalInventoryItems)
        {
            try
            {

                return normalInventoryItems.First(normalInventoryItem =>
                    GameController.Files.BaseItemTypes.Translate(normalInventoryItem.Item.Path).BaseName
                        .Equals(baseName));


                /*OLD Temporary fix, might need this sometime again
                return normalInventoryItems.First(normalInventoryItem => normalInventoryItem.Item.Path.Contains("Identification"));
                */
            }
            catch
            {
                return null;
            }
        }
    }
}