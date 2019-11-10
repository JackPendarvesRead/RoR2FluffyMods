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

        private ConfigEntry<bool> EnableLogging;
        private ConfigEntry<LogLevel> LoggingLevel;

        public void Awake()
        {
            EnableLogging = Config.AddSetting<bool>("Enable/Disable", "Enable logging", true, "Enable/Disable input logging");
            LoggingLevel = Config.AddSetting<LogLevel>("Enable/Disable", "Enable logging", LogLevel.Info, "Enable/Disable input logging");
        }

        public void Update()
        {
            var ray = new Ray(new Vector3(0,0,0), new Vector3(0,0,0));
            
            
            


            RoR2.NetworkUser.readOnlyLocalPlayersList[0].GetCurrentBody();

            if (EnableLogging.Value)
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        Logger.Log(LoggingLevel.Value, $"InputLogger: {key.ToString()} down");
                    }
                    if (Input.GetKeyUp(key))
                    {
                        Logger.Log(LoggingLevel.Value, $"InputLogger: {key.ToString()} up");
                    }
                }
            }            
        }        
    }
}
