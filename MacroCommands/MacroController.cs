using FluffyLabsConfigManagerTools.Infrastructure;
using RoR2;
using System;
using System.Linq;
using UnityEngine;

namespace MacroCommands
{
    internal class MacroController
    {
        public void ExecuteMacro(MacroConfigEntry macro)
        {
            var nu = GetNetworkUser();
            var commands = GetCommandArray(macro.MacroString);
            for (var i = 0; i < macro.RepeatCount; i++)
            {
                foreach (var command in commands)
                {
                    var cmd = GetCommandFromString(command);
                    if (!string.IsNullOrWhiteSpace(cmd.Name))
                    {
                        RoR2.Console.instance.RunClientCmd(nu, cmd.Name, cmd.Args);
                    }
                }
            }
        }

        private string[] GetCommandArray(string macroCommandString)
        {
            return macroCommandString.Split(';');
        }

        private NetworkUser GetNetworkUser()
        {
            return RoR2.LocalUserManager.GetFirstLocalUser().currentNetworkUser;
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
