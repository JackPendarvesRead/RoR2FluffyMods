using BepInEx;
using BepInEx.Configuration;
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
    [BepInPlugin("com.FluffyMods.InfusionStackFix", "InfusionStackFix", "1.3.0")]
    public class InfusionStackFix : BaseUnityPlugin
    {
        private ConfigWrapper<int> MaxHpPerInfusionStack;
        private ConfigWrapper<int> MaxHealthGainPerKill;
        private ConfigWrapper<bool> TurretReceivesBonusFromEngineer;
        private ConfigWrapper<bool> LegacyInfusion;

        public void Awake()
        {
            #region ConfigWrappers
            const string infusionSectionName = "Infusion";
            const string engineerSectionName = "Engineer";

            MaxHpPerInfusionStack = Config.Wrap<int>(                
                infusionSectionName, 
                nameof(MaxHpPerInfusionStack),
                "Set the maximum health that each infusion gives you. (Unlimited if LegacyInfusion enabled)",
                100
                );

            MaxHealthGainPerKill = Config.Wrap<int>(
                infusionSectionName, 
                nameof(MaxHealthGainPerKill),
                "Set the maximum value for health gain per kill. Set value to 0 for default mod behaviour (i.e. not limited, max=infusionStacks)",
                0                                  
                );

            LegacyInfusion = Config.Wrap<bool>(
               infusionSectionName,
               nameof(LegacyInfusion),
               "If enabled there is no maximum for infusion bonus (overrides MaxHpPerInfusionStack)",
               false
               );

            TurretReceivesBonusFromEngineer = Config.Wrap<bool>(
                engineerSectionName, 
                nameof(TurretReceivesBonusFromEngineer),
                "If enabled then turrets will receive the current infusion bonus of the Engineer on creation",
                true
                );
            #endregion

            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            On.RoR2.Inventory.AddInfusionBonus += Inventory_AddInfusionBonus;
            On.RoR2.CharacterMaster.AddDeployable += CharacterMaster_AddDeployable;
        }

        private void CharacterMaster_AddDeployable(On.RoR2.CharacterMaster.orig_AddDeployable orig,
            CharacterMaster self,
            Deployable deployable,
            DeployableSlot slot)
        {
            orig(self, deployable, slot);
            if (this.TurretReceivesBonusFromEngineer.Value
                && slot == DeployableSlot.EngiTurret)
            {
                var ownerMasterBonus = deployable.ownerMaster.inventory.infusionBonus;
                var turretMaster = deployable.GetComponent<CharacterMaster>();
                turretMaster.inventory.AddInfusionBonus(ownerMasterBonus);
            }
        }        
        
        private void Inventory_AddInfusionBonus(On.RoR2.Inventory.orig_AddInfusionBonus orig, Inventory self, uint value)
        {
            if (!LegacyInfusion.Value)
            {
                var maxInfusionBonus = self.GetItemCount(ItemIndex.Infusion) * this.MaxHpPerInfusionStack.Value;
                if (self.infusionBonus >= maxInfusionBonus)
                {
                    return;
                }
                var hpUntilMaximum = (uint)maxInfusionBonus - self.infusionBonus;
                if (hpUntilMaximum < value)
                {
                    value = hpUntilMaximum;
                }
            }
            Debug.Log($"Infusion bonus = {self.infusionBonus}");
            orig(self, value);
        }

        private void GlobalEventManager_OnCharacterDeath(MonoMod.Cil.ILContext il)
        {
            var c = new ILCursor(il);

            //Method to replace maximum bonus per infusion
            c.GotoNext(
                x => x.MatchLdloc(33), //Infusion Count
                x => x.MatchLdcI4(100),
                x => x.MatchMul()
                );
            c.Index += 1;
            c.Remove();
            c.Emit(OpCodes.Ldc_I4, MaxHpPerInfusionStack.Value);

            //Method to replace 1hp being added per infusion kill
            c.GotoNext(
                x => x.MatchLdloc(54),
                x => x.MatchLdcI4(1),
                x => x.MatchStfld(out FieldReference fr1)
                );
            c.Index += 1;
            c.Remove(); //Remove the 1
            c.Emit(OpCodes.Ldloc, (short)33);  //Infusion Count
            c.Emit(OpCodes.Ldloc, (short)14);  //Inventory
            c.EmitDelegate<Func<int, Inventory, int>>((infusionCount, inventory) =>
            {
                if (LegacyInfusion.Value)
                {
                    return infusionCount;
                }
                var maximumBonus = infusionCount * MaxHpPerInfusionStack.Value;
                var currentBonus = (int)inventory.infusionBonus;
                var hpUntilMaximum = maximumBonus - currentBonus;
                var maximumOrbGain = GetMaximumOrbValue(infusionCount);

                if (hpUntilMaximum > maximumOrbGain)
                {
                    return maximumOrbGain;
                }
                else
                {
                    return hpUntilMaximum > 0 ? hpUntilMaximum : 0;
                }
            });
        }

        private int GetMaximumOrbValue(int infusionCount)
        {
            if(MaxHealthGainPerKill.Value > 0)
            {
                return infusionCount < MaxHealthGainPerKill.Value ? infusionCount : MaxHealthGainPerKill.Value;
            }
            return infusionCount;
        }
    }
}
