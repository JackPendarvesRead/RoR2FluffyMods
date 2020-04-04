using BepInEx;
using BepInEx.Configuration;
using FluffyLabsConfigManagerTools.Infrastructure;
using FluffyLabsConfigManagerTools.Util;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace InfusionStackFix
{
    
    [BepInDependency(FluffyLabsConfigManagerTools.FluffyConfigLabsPlugin.PluginGuid)]
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class InfusionStackFix : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "InfusionStackFix";
        private const string pluginVersion = "5.0.0";

        private ConditionalConfigEntry<uint> MaximumHealthPerInfusion;
        private ConditionalConfigEntry<uint> MaxHealthGainPerKill;
        private ConfigEntry<bool> TurretReceivesBonusFromEngineer;
        private ConfigEntry<bool> TurretGivesEngineerLifeOrbs;

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
            MaximumHealthPerInfusion = conditionalUtil.AddConditionalConfig<uint>(
                infusionSectionName,
                nameof(MaximumHealthPerInfusion),
                100,
                true,
                new ConfigDescription("Maximum health gained per infusion. Disable for no limit."));

            MaxHealthGainPerKill = conditionalUtil.AddConditionalConfig<uint>(
                infusionSectionName,
                nameof(MaxHealthGainPerKill),
                5,
                false,
                new ConfigDescription(
                    "Enable to set the maximum value for health gain per kill."
                    )
                );

            TurretReceivesBonusFromEngineer = Config.Bind<bool>(
                engineerSectionName,
                nameof(TurretReceivesBonusFromEngineer),
                true,
                new ConfigDescription(
                    "If enabled then turrets will receive the current infusion bonus of the Engineer on creation"
                    )
                );

            TurretGivesEngineerLifeOrbs = Config.Bind<bool>(
                engineerSectionName,
                nameof(TurretGivesEngineerLifeOrbs),
                true,
                new ConfigDescription(
                    "If enabled the main engineer body will receive an infusion orb whenever a turret he owns makes a kill"
                    )
                );
            #endregion
                        
            On.RoR2.Inventory.AddInfusionBonus += Inventory_AddInfusionBonus;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            On.RoR2.CharacterMaster.AddDeployable += CharacterMaster_AddDeployable;
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            var attacker = damageReport.attackerMaster;
            if (TurretGivesEngineerLifeOrbs.Value &&
                attacker.name.ToLower().Contains("turret"))
            {
                attacker.minionOwnership.ownerMaster.inventory.AddInfusionBonus(1);                
            }
            orig(self, damageReport);
        }

        private void CharacterMaster_AddDeployable(On.RoR2.CharacterMaster.orig_AddDeployable orig,
            CharacterMaster self,
            Deployable deployable,
            DeployableSlot slot)
        {
            orig(self, deployable, slot);
            if (TurretReceivesBonusFromEngineer.Value &&
                slot == DeployableSlot.EngiTurret)
            {
                var ownerMasterBonus = deployable.ownerMaster.inventory.infusionBonus;
                if(ownerMasterBonus > 0)
                {
                    var turretMaster = deployable.GetComponent<CharacterMaster>();
                    turretMaster.inventory.AddInfusionBonus(ownerMasterBonus);
                }
                
            }
        }

        private void Inventory_AddInfusionBonus(On.RoR2.Inventory.orig_AddInfusionBonus orig, Inventory self, uint bonusGained)
        {
            if(bonusGained == 1)
            {
                bonusGained = RecalculateBonusGain(self);
            }
            orig(self, bonusGained);
        }

        private uint RecalculateBonusGain(Inventory self)
        {
            uint infusionCount = (uint)self.GetItemCount(ItemIndex.Infusion);
            uint maximumBonus = infusionCount * MaximumHealthPerInfusion.Value;
            uint currentBonus = self.infusionBonus;
            uint lifeUntilMaximum = GetBonusUntilMaximum(maximumBonus, currentBonus);
            uint maximumBonusGain = GetMaximumBonusGain(infusionCount);

            if (lifeUntilMaximum > maximumBonusGain)
            {
                return maximumBonusGain;
            }
            else
            {
                return lifeUntilMaximum;
            }
        }

        private uint GetMaximumBonusGain(uint infusionCount)
        {
            if (MaxHealthGainPerKill.Condition &&
                infusionCount > MaxHealthGainPerKill.Value)
            {
                return MaxHealthGainPerKill.Value;
            }
            else
            {
                return infusionCount;
            }
        }

        private uint GetBonusUntilMaximum(uint maxBonus, uint currentBonus)
        {
            if(currentBonus >= maxBonus)
            {
                return 0;
            }
            else
            {
                return maxBonus - currentBonus;
            }
        }
    }
}
