using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace BackupEngiTurrets
{
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class BackupEngiTurrets : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "BackupEngiTurrets";
        private const string pluginVersion = "3.0.0";

        private static ConfigEntry<bool> TurretStockIncreasesWithBackup;
        private static ConfigEntry<bool> PrimaryIncreasesWitBackup;

        public void Awake()
        {
            TurretStockIncreasesWithBackup = Config.Bind<bool>(
                                   new ConfigDefinition("Engineer", nameof(TurretStockIncreasesWithBackup)),
                                   true,
                                   new ConfigDescription(
                                       "Enable to set Place Turret (R) ability to gain stock with BackupMagazine."
                                       )
                                   );

            PrimaryIncreasesWitBackup = Config.Bind<bool>(
                                   new ConfigDefinition("Mage", nameof(PrimaryIncreasesWitBackup)),
                                   false,
                                   new ConfigDescription(
                                       "Enable to set Artificier primary (LMB) ability to gain stock with BackupMagazine."
                                       )
                                   );

            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            try
            {
                if (self.name.StartsWith(CharBodyStrings.Engineer))
                {
                    if (TurretStockIncreasesWithBackup.Value)
                    {
                        self.GetComponent<SkillLocator>().special.SetBonusStockFromBody(self.inventory.GetItemCount(ItemIndex.SecondarySkillMagazine));
                    }
                }
                if (self.name.StartsWith(CharBodyStrings.Mage))
                {
                    if (PrimaryIncreasesWitBackup.Value)
                    {
                        self.GetComponent<SkillLocator>().primary.SetBonusStockFromBody(self.inventory.GetItemCount(ItemIndex.SecondarySkillMagazine));
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex);
            }
            orig(self);
        }
    }
}
