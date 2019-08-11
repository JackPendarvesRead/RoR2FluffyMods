using BepInEx;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RoR2FluffyMods
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.RoR2FluffyMods", "RoR2FluffyMods", "0.0.0")]
    public class TestingStuff : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.Console.Awake += (orig, self) =>
            {
                CommandHelper.RegisterCommands(self);
                orig(self);
            };

            

            foreach(var plugin in BepInEx.Bootstrap.Chainloader.Plugins)
            {
                var x = MetadataHelper.GetMetadata(plugin);
                Debug.Log($"PLUGINNAME: {x.Name}");
            }
        }
    }
}
