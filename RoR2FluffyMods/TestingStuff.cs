using BepInEx;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2FluffyMods
{
    [BepInPlugin("com.TEST.TEST", "TEST", "0.0.0")]
    public class TestingStuff : BaseUnityPlugin
    {
        System.Random rng;

        public void Awake()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            rng = new System.Random();
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, 
            GlobalEventManager self, 
            DamageInfo damageInfo, 
            GameObject victim)
        {
            var body = victim.GetComponent<CharacterBody>();
            if (body.isPlayerControlled)
            {
                var itemsInInventory = GetAllItems(body.inventory).ToList();
                if(itemsInInventory.Count > 0)
                {
                    var selectedIndex = rng.Next(itemsInInventory.Count);
                    var itemToRemove = itemsInInventory[selectedIndex];
                    body.inventory.RemoveItem(itemToRemove);
                }
            }
            orig(self, damageInfo, victim);
        }

        private IEnumerable<ItemIndex> GetAllItems(Inventory inventory)
        {
            var items = RoR2.ItemCatalog.allItems
                .Where(i => RoR2.ItemCatalog.GetItemDef(i).tier != ItemTier.NoTier);

            foreach (var item in items)
            {                
                var n = inventory.GetItemCount(item);
                if(n > 0)
                {
                    for(var i = 0; i < n; i++)
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
