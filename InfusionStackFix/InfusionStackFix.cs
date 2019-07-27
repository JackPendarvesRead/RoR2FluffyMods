using BepInEx;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Utils;
using RoR2;
using System;
using UnityEngine;

namespace InfusionStackFix
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.InfusionStackFix", "InfusionStackFix", "1.0.0")]
    public class InfusionStackFix : BaseUnityPlugin
    {
        public void Awake()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            On.RoR2.Inventory.AddInfusionBonus += Inventory_AddInfusionBonus;
        }

        private void Inventory_AddInfusionBonus(On.RoR2.Inventory.orig_AddInfusionBonus orig, Inventory self, uint value)
        {
            var max = self.GetItemCount(ItemIndex.Infusion) * 100;
            if(self.infusionBonus >= max)
            {
                return;
            }
            orig(self, value);
        }

        private void GlobalEventManager_OnCharacterDeath(MonoMod.Cil.ILContext il)
        {            
            var c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdloc(51),
                x => x.MatchLdcI4(1),
                x => x.MatchStfld(out FieldReference fr1)
                );
            c.Index += 1;
            c.Remove(); //Remove the 1
            c.Emit(OpCodes.Ldloc, (short)29);  //Infusion Count
            c.Emit(OpCodes.Ldloc, (short)22);  //Inventory
            c.Emit(OpCodes.Callvirt, typeof(Inventory).GetProperty("infusionBonus").GetGetMethod());
            c.EmitDelegate<Func<int, uint, int>>((infusionCount, bonus) =>
            {
                int maximumBonus = infusionCount * 100;
                int currentBonus = (int)bonus;
                if(maximumBonus - currentBonus > infusionCount)
                {
                    return infusionCount;
                }
                else
                {
                    return maximumBonus - currentBonus > 0 ? maximumBonus - currentBonus : 0;
                }
            });
        }
    }
}
