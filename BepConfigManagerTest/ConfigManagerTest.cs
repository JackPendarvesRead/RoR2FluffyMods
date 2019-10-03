using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using BepInEx.Configuration;
using UnityEngine;

namespace BepConfigManagerTest
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.ConfigManagerTest", "ConfigManagerTest", "0.0.0")]
    public class ConfigManagerTest : BaseUnityPlugin
    {
        private static ConfigEntry<bool> MyBoolConfig;
        private static ConfigEntry<float> MyFloatConfig;

        public void Awake()
        {
            const string sectionName = "MySection";

            MyBoolConfig = Config.AddSetting<bool>(
                new ConfigDefinition(sectionName, nameof(MyBoolConfig)),
                false,
                new ConfigDescription(
                    "My bool config"
                    ));

            MyFloatConfig = Config.AddSetting<float>(
               new ConfigDefinition(sectionName, nameof(MyBoolConfig)),
               10.0f,
               new ConfigDescription(
                   "My float config"
                   ));
        }
    }
}
