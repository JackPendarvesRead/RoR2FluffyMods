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
    [BepInPlugin("com.FluffyMods.MacroCommands", "MacroCommands", "1.0.0")]
    public class MacroCommands : BaseUnityPlugin
    {
        private List<MacroConfigEntry> Macros;

        public void Start()
        {            
            Macros = GetMacros().ToList();
            var bUtil = new ButtonUtil(this.Config);
            bUtil.AddButtonConfig("Advanced", 
                "Add Macro", 
                "Press this button to add additional macro ConfigEntry", 
                GetButtonDictionary(),
                false,
                new ConfigurationManagerAttributes { HideDefaultButton = true, IsAdvanced = true });
        }

        private Dictionary<string, Action> GetButtonDictionary()
        {
            return new Dictionary<string, Action>
            {
                { "Add", AddNewMacro }
            };
        }

        private const string macroSection = "Macros";
        private readonly int n = 5;
        private IEnumerable<MacroConfigEntry> GetMacros()
        {
            var mUtil = new MacroUtil(this.Config);
            for (var i = 0; i < n; i++)
            {
                var number = (i + 1).ToString("00");
                yield return mUtil.AddMacroConfig(macroSection, $"Macro {number}", "Type Macro into the black box. Commands are seperated by ';'");
            }
        }

        private void AddNewMacro()
        {
            if(Macros.Count < 100)
            {
                var mUtil = new MacroUtil(this.Config);
                var number = (Macros.Count + 1).ToString("00");
                var macro = mUtil.AddMacroConfig
                    (macroSection, 
                    $"Macro {number}", 
                    "Type Macro into the black box. Commands are seperated by ';'", 
                    new ConfigurationManagerAttributes { IsAdvanced = true, HideDefaultButton = true });
                Macros.Add(macro);
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
