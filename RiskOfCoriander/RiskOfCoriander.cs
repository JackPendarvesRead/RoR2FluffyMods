﻿using BepInEx;
using BepInEx.Configuration;
using System;
using UnityEngine;

namespace RiskOfCoriander
{
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class RiskOfCoriander : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "RiskOfCoriander";
        private const string pluginVersion = "1.0.0";

        internal static ConfigEntry<CorianderType> CorianderFreshness;
        internal static ConfigEntry<float> Coarseness;

        public void Awake()
        {
            const string section = "Coriander";

            CorianderFreshness = Config.Bind(
                section,
                "CorianderType",
                CorianderType.Fresh,
                new ConfigDescription("Select type of coriander"));

            Coarseness = Config.Bind(
                section,
                "Coarseness",
                0.5f,
                new ConfigDescription("How coarse should the coriander be (as a %)", new AcceptableValueRange<float>(0f, 1f)));

            On.RoR2.CharacterMaster.SpawnBody += CharacterMaster_SpawnBody;
        }

        private RoR2.CharacterBody CharacterMaster_SpawnBody(On.RoR2.CharacterMaster.orig_SpawnBody orig, RoR2.CharacterMaster self, UnityEngine.GameObject bodyPrefab, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation)
        {
            try
            {
                self.gameObject.AddCorriander();
                //Logger.LogInfo("CORRIANDER ADDED YAY!");
            }
            catch
            {
                Logger.LogDebug("There was a problem with adding corriander but shhh don't tell anyone...");
            }   
            return orig(self, bodyPrefab, position, rotation);
        }
    }  
}
