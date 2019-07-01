using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace BackupEngiTurrets
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.TestingStuff", "TestingStuff", "0.0.0")]
    public class TestingStuff : BaseUnityPlugin
    {
        private static ConfigWrapper<bool> config;

        public void Awake()
        {
            config = Config.Wrap(                                  
                "section",                                  
                "key",                                 
                "desctiption.",                                  
                true                                   
                );

            //On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }
    }
}
