using PoeHUD.Models;
using PoeHUD.Plugins;
using PoeHUD.Framework;
using PoeHUD.Models.Enums;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.Elements;
using PoeHUD.Poe.RemoteMemoryObjects;
using SharpDX;
using System.Threading;
using UnIdy.Utils;
using System.Windows.Forms;

namespace UnIdy
{
    class UnIdy:BaseSettingsPlugin<Settings>
    {
        private Inventory playerInventory;
        private IngameState ingameState;
        private bool isBusy = false;
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
            {
                return;
            }

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
                LogMessage("Open your player inventory first!",5);
                return;
            }

            playerInventory = ingameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory];
            var playerInventoryItems = playerInventory.VisibleInventoryItems;
            Vector2 scrollPosition = getScrollsPosition();

            Mouse.moveMouse(scrollPosition);


            Keyboard.HoldKey((byte)Keys.ShiftKey);
            Mouse.RightUp(Settings.Speed);
            foreach (NormalInventoryItem item in playerInventoryItems)
            {
                Mods itemMods = item.Item.GetComponent<Mods>();
                if (!itemMods.Identified 
                    && 
                    (
                    (Settings.Rare && itemMods.ItemRarity == ItemRarity.Rare) 
                    || 
                    (Settings.Magic && itemMods.ItemRarity == ItemRarity.Magic)
                    ||
                    (Settings.Unique && itemMods.ItemRarity == ItemRarity.Unique)
                    )
                    )
                {
                    var itemPosition = item.GetClientRect().Center;
                    identifyItem(scrollPosition,itemPosition);
                }
            }
            Keyboard.ReleaseKey((byte)Keys.ShiftKey);

        }

        private Vector2 getScrollsPosition()
        {
            playerInventory = ingameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory];
            var playerInventoryItems = playerInventory.VisibleInventoryItems;
            foreach (NormalInventoryItem item in playerInventoryItems)
            {
                Mods itemMods = item.Item.GetComponent<Mods>();
                if (item.Item.Path.Contains("CurrencyIdentification")) { 
                return item.GetClientRect().Center;
                }
            }
            return new Vector2(0,0);
        }
        private void identifyItem(Vector2 scrollPosition,Vector2 itemPosition)
        {
            Thread.Sleep(Mouse.DELAY_MOVE);
            //Mouse.moveMouse(scrollPosition);
            //Mouse.RightUp(Settings.Speed);
            Mouse.moveMouse(itemPosition);
            Mouse.LeftUp(Settings.Speed);
            Thread.Sleep(Mouse.DELAY_MOVE);
        }
    }
}
