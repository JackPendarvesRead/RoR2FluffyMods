using BepInEx;
using BepInEx.Configuration;
using FluffyLabsConfigManagerTools.Infrastructure;
using FluffyLabsConfigManagerTools.Util;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace InfusionStackFix
{
    [BepInDependency("com.FluffyMods.FluffyLabsConfigManagerTools")]
    [BepInPlugin("com.FluffyMods.InfusionStackFix", "InfusionStackFix", "2.0.0")]
    public class InfusionStackFix : BaseUnityPlugin
    {
        private ConditionalConfigEntry<int> MaximumHealthPerInfusion;
        private ConditionalConfigEntry<int> MaxHealthGainPerKill;
        private ConfigEntry<bool> TurretReceivesBonusFromEngineer;

        public void Awake()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }

            #region ConfigWrappers
            const string infusionSectionName = "Infusion";
            const string engineerSectionName = "Engineer";

            var conditionalUtil = new ConditionalUtil(this.Config);
            MaximumHealthPerInfusion = conditionalUtil.AddConditionalConfig<int>(
                infusionSectionName,
                nameof(MaximumHealthPerInfusion),
                100,
                true,
                new ConfigDescription("Maximum health gained per infusion. Disable for no limit."));

            MaxHealthGainPerKill = conditionalUtil.AddConditionalConfig<int>(
                infusionSectionName,
                nameof(MaxHealthGainPerKill),
                5,
                false,
                new ConfigDescription(
                    "Enable to set the maximum value for health gain per kill."
                    )
                );

            TurretReceivesBonusFromEngineer = Config.AddSetting<bool>(
                engineerSectionName,
                nameof(TurretReceivesBonusFromEngineer),
                true,
                new ConfigDescription(
                    "If enabled then turrets will receive the current infusion bonus of the Engineer on creation"
                    )
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
            if (MaximumHealthPerInfusion.Condition)
            {
                var maxInfusionBonus = self.GetItemCount(ItemIndex.Infusion) * MaximumHealthPerInfusion.Value;
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
            c.EmitDelegate<Func<int>>(() =>
            {
                if (MaxHealthGainPerKill.Condition)
                {
                    return MaximumHealthPerInfusion.Value;
                }
                else
                {
                    return int.MaxValue;
                }
            });

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
                if (!MaximumHealthPerInfusion.Condition)
                {
                    return infusionCount;
                }
                var maximumBonus = infusionCount * MaximumHealthPerInfusion.Value;
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
            if (MaxHealthGainPerKill.Condition
                && infusionCount > MaxHealthGainPerKill.Value)
            {
                return MaxHealthGainPerKill.Value;
            }
            return infusionCount;
        }
    }
}
