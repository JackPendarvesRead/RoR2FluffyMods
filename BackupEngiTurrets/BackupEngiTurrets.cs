using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace BackupEngiTurrets
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.BackupEngiTurrets", "BackupEngiTurrets", "2.0.2")]
    public class BackupEngiTurrets : BaseUnityPlugin
    {
        private static ConfigWrapper<bool> isTurretIncreased;
        private static ConfigWrapper<bool> isFlameBlastIncreased;

        public void Awake()
        {            
            isTurretIncreased = Config.Wrap(
                                   "Engineer",
                                   "IsTurretIncreased",
                                   "Set to true to set Place Turret (R) ability to gain stock with BackupMagazine.",
                                   true
                                   );

            isFlameBlastIncreased = Config.Wrap(
                                   "Mage",
                                   "IsFlameBlastIncreased",
                                   "Set to true to set Mage Flame Blast (LMB) ability to gain stock with BackupMagazine.",
                                   false
                                   );

            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            try
            {
                if (self.name == CharBodyStrings.Engineer)
                {
                    if (isTurretIncreased.Value)
                    {
                        self.GetComponent<SkillLocator>().special.SetBonusStockFromBody(self.inventory.GetItemCount(ItemIndex.SecondarySkillMagazine));
                    }
                }
                if(self.name == CharBodyStrings.Mage)
                {
                    if (isFlameBlastIncreased.Value)
                    {
                        self.GetComponent<SkillLocator>().primary.SetBonusStockFromBody(self.inventory.GetItemCount(ItemIndex.SecondarySkillMagazine));
                    }
                }
            }
            catch
            {
                Logger.LogError("CharacterBody_RecalculateStats failed.");
            }
            finally
            {
                orig(self);
            }
        }      
    }
}
