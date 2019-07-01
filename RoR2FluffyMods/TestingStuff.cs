using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace BackupEngiTurrets
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.BackupEngiTurrets", "BackupEngiTurrets", "2.0.1")]
    public class TestingStuff : BaseUnityPlugin
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

            //On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }
    }
}
