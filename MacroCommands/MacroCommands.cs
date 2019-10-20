﻿using BepInEx;
using FluffyLabsConfigManagerTools.Infrastructure;
using FluffyLabsConfigManagerTools.Util;
using RoR2;
using System.Collections.Generic;
using System.Linq;
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
        }

        public void Update()
        {
            foreach (var macro in Macros.Where(m => m.KeyboardShortcut.MainKey != KeyCode.None))
            {                
                if (Input.GetKeyDown(macro.KeyboardShortcut.MainKey))
                {
                    for(var i = 0; i < macro.RepeatCount; i++)
                    {
                        ExecuteMacro(macro);
                    }
                }
            }
        }

        int n = 5;
        private IEnumerable<MacroConfigEntry> GetMacros()
        {
            const string macroSection = "Macros";
            var c = new MacroUtil(this.Config);
            for (var i = 0; i < n; i++)
            {
                var number = (i + 1).ToString("00");
                yield return c.AddMacroConfig(macroSection, $"Macro {number}", $"This is macro {number}");
            }
        }

        private void ExecuteMacro(MacroConfigEntry macro)
        {
            var nu = GetNetworkUser();
            var commands = GetCommands(macro.MacroString);
            foreach(var command in commands)
            {                
                var cmd = GetCommandFromString(command);
                if (!string.IsNullOrWhiteSpace(cmd.Name))
                {
                    Debug.Log($"Running command: {command.Trim()}");
                    Console.instance.RunClientCmd(nu, cmd.Name, cmd.Args);
                }
            }
        }

        private string[] GetCommands(string macroCommandString)
        {
            return macroCommandString.Split(';');
        }

        private NetworkUser GetNetworkUser()
        {
            return RoR2.NetworkUser.readOnlyInstancesList[0];
        }

        private Command GetCommandFromString(string commandString)
        {
            var split = commandString.Trim().Split(' ');
            var name = split[0];
            var args = split.Skip(1).ToArray<string>();
            return new Command
            {
                Name = name,
                Args = args
            };
        }
    }
}
