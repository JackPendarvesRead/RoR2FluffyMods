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
