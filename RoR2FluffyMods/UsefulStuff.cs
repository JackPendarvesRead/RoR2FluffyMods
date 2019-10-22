using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RoR2FluffyMods
{
    class Stuff
    {
        public IEnumerable<BepInPlugin> GetPlugins()
        {
            foreach (var plugin in BepInEx.Bootstrap.Chainloader.Plugins)
            {
                yield return MetadataHelper.GetMetadata(plugin);
            }
        }
    }
}

//[ConCommand(commandName = "GiveLunarCoins", flags = ConVarFlags.ExecuteOnServer, helpText = "Help text goes here")]
//private static void GiveLunarCoins(ConCommandArgs args)
//{
//    var n = uint.Parse(args[0]);
//    foreach (var nu in NetworkUser.readOnlyInstancesList)
//    {
//        nu.AwardLunarCoins(n);
//    }
//}
