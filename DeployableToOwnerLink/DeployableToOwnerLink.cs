using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
using System.Reflection;
using System;
using System.Collections.Generic;
using DeployableOwnerInformation.Component;

namespace DeployableOwnerInformation
{
    [PluginMetadata(PluginGuid, pluginName, pluginVersion)]
    public class DeployableOwnerInformation : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "DeployableOwnerInformation";
        private const string pluginVersion = "1.0.0";

        public void Start()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }
            
            IL.RoR2.MasterSummon.Perform += MasterSummon_Perform;
        }

        private void MasterSummon_Perform(ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(x => x.MatchLdloc((byte)4));
            c.Emit(OpCodes.Ldloc_1);  // sumoner body
            c.Emit(OpCodes.Ldloc, (short)4); // summoned character master
            c.EmitDelegate<Action<CharacterBody, CharacterMaster>>((summoner, cm) =>
            {
                if (summoner != null && cm != null)
                {
                    var oi = cm.gameObject.AddComponent<OwnerInformation>();
                    oi.OwnerBody = summoner;
                }
            });
        }
    }
}
