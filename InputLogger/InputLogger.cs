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
    public class InputLogger : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "InputLogger";
        private const string pluginVersion = "1.0.0";

        private ConfigEntry<bool> EnableLogging;
        private ConfigEntry<bool> LogKeyUp;
        private ConfigEntry<bool> LogKeyDown;
        private ConfigEntry<LogLevel> LogLevelSelected;
        private ConfigEntry<KeyboardShortcut> kbs;

        public void Awake()
        {
            const string logSection = "Logging";

            LogLevelSelected = Config.Bind<LogLevel>(
                logSection, 
                "Log Level", 
                LogLevel.Info, 
                "Select the level at which logs should be logged at"
                );           

            EnableLogging = Config.Bind<bool>(
                "Enable/Disable", 
                "Enable logging", 
                true, 
                "Enable/Disable input logging"
                );

            LogKeyUp = Config.Bind<bool>(
                logSection, 
                "Log Key Up", 
                true, 
                "Enable/Disable logging on key up"
                );

            LogKeyDown = Config.Bind<bool>(
                logSection,
                "Log Key Down", 
                true, 
                "Enable/Disable logging on key down"
                );

            kbs = Config.Bind<KeyboardShortcut>(
                "Keyboard Shortcut", 
                "KBS To Test", 
                new KeyboardShortcut(KeyCode.None), 
                new ConfigDescription(
                    "If you want to test a keyboard shortcut is being triggered input the shortcut here and it will be logged when triggered")
                    );
        }

        public void Update()
        {
            if (EnableLogging.Value)
            {                
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (LogKeyDown.Value && Input.GetKeyDown(key))
                    {
                        Logger.Log(LogLevelSelected.Value, $"InputLogger: {key.ToString()} down");
                    }
                    if (LogKeyUp.Value && Input.GetKeyUp(key))
                    {
                        Logger.Log(LogLevelSelected.Value, $"InputLogger: {key.ToString()} up");
                    }
                }
                if (kbs.Value.MainKey != KeyCode.None)
                {
                    if (kbs.Value.IsUp())
                    {
                        Logger.Log(LogLevelSelected.Value, "Test shortcut was triggered");
                    }
                }
            }
        }
    }
}
