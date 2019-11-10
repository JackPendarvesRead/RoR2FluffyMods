using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2FluffyMods
{
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class TestingStuff : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "InputLogger";
        private const string pluginVersion = "1.0.0";

        private ConfigEntry<bool> EnableLogging;

        public void Awake()
        {
            EnableLogging = Config.Bind<bool>("Enable/Disable", "Enable logging", true, "Enable/Disable input logging");
        }

        public void Update()
        {
            if (EnableLogging.Value)
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        Logger.LogInfo($"InputLogger: {key.ToString()} down");
                    }
                    if (Input.GetKeyUp(key))
                    {
                        Logger.LogInfo($"InputLogger: {key.ToString()} up");
                    }
                }
            }
        }
    }
}
