using BepInEx;
using Mono.Cecil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoftDeps
{
    [BepInDependency(AAA.PluginGuid, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class CCC : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "CCC";
        private const string pluginVersion = "1.0.0";

        public void Awake()
        {
            Debug.Log(pluginName + " has loaded");
        }
    }
}
