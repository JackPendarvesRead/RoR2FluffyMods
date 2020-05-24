using BepInEx;
using CustomCharacterPlay.HelperStuff;
using EntityStates.MyCustomCharacter;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomCharacterPlay
{
    [R2APISubmoduleDependency(nameof(R2API.SurvivorAPI))]
    [R2APISubmoduleDependency(nameof(R2API.PrefabAPI))]
    [R2APISubmoduleDependency(nameof(R2API.LoadoutAPI))]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class CustomCharacterPlay : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "CustomCharacterPlay";
        private const string pluginVersion = "1.0.0";

        public void Awake()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }

            var characterInfo = new CharacterInformation("Whatever", "This is the description");
            var bodyInfo = new PrefabInfo("BodyName", PrefabCollection.CommandoBody);
            var displayInfo = new PrefabInfo("DisplayName", PrefabCollection.CommandoDisplay);
            new CharacterCreator().Create<MyCustomCharacter>(characterInfo, bodyInfo, displayInfo);
        }       
    }
}
