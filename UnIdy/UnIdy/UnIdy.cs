using System.Threading;
using System.Windows.Forms;
using PoeHUD.Framework;
using PoeHUD.Models;
using PoeHUD.Models.Enums;
using PoeHUD.Plugins;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.RemoteMemoryObjects;
using SharpDX;
using UnIdy.Utils;

namespace UnIdy
{
    internal class UnIdy : BaseSettingsPlugin<Settings>
    {
        private readonly IngameState ingameState;
        private bool isBusy;
        private Inventory playerInventory;

        public UnIdy()
        {
            ingameState = GameController.Game.IngameState;
            PluginName = "UnIdy";
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Render()
        {
            if (!Settings.Enable)
                return;


            debug();

            if (WinApi.IsKeyDown(Settings.HotKey) && !isBusy)
            {
                isBusy = true;
                scanInventory();
                isBusy = false;
            }
        }

        public override void EntityAdded(EntityWrapper entityWrapper)
        {
            base.EntityAdded(entityWrapper);
        }

        public override void EntityRemoved(EntityWrapper entityWrapper)
        {
            base.EntityRemoved(entityWrapper);
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        private void scanInventory()
        {
            if (!ingameState.IngameUi.InventoryPanel.IsVisible)
            {
                LogMessage("Open your player inventory first!", 5);
                return;
            }

            playerInventory = ingameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory];
            var playerInventoryItems = playerInventory.VisibleInventoryItems;

            var prevMousePosition = Mouse.GetCursorPosition();
            var scrollPosition = getScrollsPosition();


            Mouse.moveMouse(scrollPosition);


            Keyboard.HoldKey((byte) Keys.ShiftKey);
            Mouse.RightUp(Settings.Speed);
            foreach (var item in playerInventoryItems)
            {
                var itemMods = item.Item.GetComponent<Mods>();
                if (!itemMods.Identified
                    &&
                    (
                        Settings.Rare && itemMods.ItemRarity == ItemRarity.Rare && item.Item.GetComponent<Map>() == null
                        ||
                        Settings.Magic && itemMods.ItemRarity == ItemRarity.Magic &&
                        item.Item.GetComponent<Map>() == null
                        ||
                        Settings.Unique && itemMods.ItemRarity == ItemRarity.Unique &&
                        item.Item.GetComponent<Map>() == null
                        ||
                        Settings.Map && itemMods.ItemRarity != ItemRarity.Normal &&
                        item.Item.GetComponent<Map>() != null
                    )
                )
                {
                    var itemPosition = item.GetClientRect().Center;
                    identifyItem(scrollPosition, itemPosition);
                }
            }
            Keyboard.ReleaseKey((byte) Keys.ShiftKey);
            Mouse.moveMouse(prevMousePosition);
        }

        private Vector2 getScrollsPosition()
        {
            playerInventory = ingameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory];
            var playerInventoryItems = playerInventory.VisibleInventoryItems;
            foreach (var item in playerInventoryItems)
            {
                var itemMods = item.Item.GetComponent<Mods>();
                if (item.Item.Path.Contains("CurrencyIdentification")) return item.GetClientRect().Center;
            }
            return new Vector2(0, 0);
        }

        private void identifyItem(Vector2 scrollPosition, Vector2 itemPosition)
        {
            Thread.Sleep(Mouse.DELAY_MOVE);
            Mouse.moveMouse(itemPosition);
            Mouse.LeftUp(Settings.Speed);
            Thread.Sleep(Mouse.DELAY_MOVE);
        }

        private void debug()
        {
            if (!Settings.Debug)
                return;
            var uihover = ingameState.UIHover.GetClientRect();
            Graphics.DrawFrame(uihover, 2, Color.Green);
            Graphics.DrawFrame(ingameState.UIHover.Parent.GetClientRect(), 2, Color.Green);
            Graphics.DrawFrame(ingameState.UIHover.Parent.Parent.GetClientRect(), 2, Color.Green);
            Graphics.DrawFrame(ingameState.UIHover.Parent.Parent.GetClientRect(), 2, Color.Green);
            Graphics.DrawFrame(ingameState.UIHover.Parent.Parent.GetClientRect(), 2, Color.Green);
            Graphics.DrawFrame(ingameState.UIHover.Parent.Parent.GetClientRect(), 2, Color.Green);

            /**
            int i = 0;
            foreach (var ui in ingameState.IngameUi.InventoryPanel.Children)
            {
                if (ui.IsVisible)
                {
                    Graphics.DrawFrame(ui.GetClientRect(), 2, Color.Red);
                    Graphics.DrawText(i.ToString(), 24, ui.GetClientRect().Center);
                }
                i++;
            }
            i = 0;
            */
        }
    }
}