using BepInEx;
using FluffyLabsConfigManagerTools.Infrastructure;
using FluffyLabsConfigManagerTools.Util;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace MacroCommands
{
    [PluginDependency(FluffyLabsConfigManagerTools.FluffyConfigLabsPlugin.PluginGuid)]
    [PluginMetadata(PluginGuid, pluginName, pluginVersion)]
    public class MacroCommands : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "MacroCommands";
        private const string pluginVersion = "1.0.0";

        private List<MacroConfigEntry> Macros;
        
        public void Start()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }
            Macros = GetMacros().ToList();
        }      

        private const string macroSection = "Macros";
        private IEnumerable<MacroConfigEntry> GetMacros()
        {
            var mUtil = new MacroUtil(this.Config);
            for (var i = 0; i < 10; i++)
            {
                var number = (i + 1).ToString("00");
                if(i < 5)
                {
                    yield return mUtil.AddMacroConfig(
                        macroSection, 
                        $"Macro {number}", 
                        "Type Macro into the black box. Commands are seperated by ';'");
                }
                else
                {
                    yield return mUtil.AddMacroConfig(
                        macroSection, 
                        $"Macro {number}", 
                        "Type Macro into the black box. Commands are seperated by ';'", 
                        new ConfigurationManagerAttributes { IsAdvanced = true, HideDefaultButton = true });
                }
            }
        }     

        public void Update()
        {
            foreach (var macro in Macros.Where(m => m.KeyboardShortcut.MainKey != KeyCode.None))
            {
                if (macro.KeyboardShortcut.IsUp())
                {
                    new MacroController().ExecuteMacro(macro);
                }
            }
        }        
    }
}
