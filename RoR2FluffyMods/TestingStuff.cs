using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2FluffyMods
{
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class InputLogger : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "InputLogger";
        private const string pluginVersion = "1.0.0";

        public void Awake()
        {
        }       
    }
}
