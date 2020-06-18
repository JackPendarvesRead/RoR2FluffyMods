using BepInEx;
using Mono.Cecil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;

namespace WildCardItem
{

    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class WildCardItem : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "WildCardItem";
        private const string pluginVersion = "1.0.0";

        private Dictionary<Inventory, ItemIndex> lastItemPickedUpDic = new Dictionary<Inventory, ItemIndex>();

        public void Awake()
        {
            On.RoR2.Inventory.GetItemCount += Inventory_GetItemCount;
            On.RoR2.GenericPickupController.AttemptGrant += GenericPickupController_AttemptGrant;
        }

        private void GenericPickupController_AttemptGrant(On.RoR2.GenericPickupController.orig_AttemptGrant orig, GenericPickupController self, CharacterBody body)
        {
            var pickupDef = PickupCatalog.GetPickupDef(self.pickupIndex);
            if(pickupDef.itemIndex != ItemIndex.None)
            {
                lastItemPickedUpDic[body.inventory] = pickupDef.itemIndex;
            }
            orig(self, body);
        }

        private int Inventory_GetItemCount(On.RoR2.Inventory.orig_GetItemCount orig, Inventory self, ItemIndex itemIndex)
        {
            if (lastItemPickedUpDic.ContainsKey(self) && lastItemPickedUpDic[self] == itemIndex)
            {
                int[] itemStackArray = ItemCatalog.RequestItemStackArray();
                int itemCount = itemStackArray[(int)itemIndex];                
                int wildCount = itemStackArray[(int)ItemIndex.Count];
                return itemCount + wildCount;
            }
            else
            {
                return orig(self, itemIndex);
            }
        }
    }   
}
