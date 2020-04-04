using BepInEx;
using MonoMod.Cil;
using RoR2;
using Mono.Cecil.Cil;
using System;
using DeployableOwnerInformation.Component;

namespace DeployableOwnerInformation
{
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class DeployableOwnerInformation : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "DeployableOwnerInformation";
        private const string pluginVersion = "1.0.2";

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
            c.GotoNext(
                x => x.MatchLdloc(5),
                x => x.MatchRet());
            c.Emit(OpCodes.Ldloc, (byte)1); // Summoner Body
            c.Emit(OpCodes.Ldloc, (byte)5); // Summoned Master
            c.EmitDelegate<Action<CharacterMaster, CharacterBody>>((master, summoner) =>
            {
                if (summoner != null && master != null)
                {
                    var oi = master.gameObject.AddComponent<OwnerInformation>();
                    oi.OwnerBody = summoner;
                }
            });

            //c.GotoNext(x => x.MatchLdloc((byte)4));
            //c.Emit(OpCodes.Ldloc_1);  // sumoner body
            //c.Emit(OpCodes.Ldloc, (short)4); // summoned character master
            //c.EmitDelegate<Action<CharacterBody, CharacterMaster>>((summoner, cm) =>
            //{
            //    if (summoner != null && cm != null)
            //    {
            //        var x = cm.minionOwnership;
            //        var oi = cm.gameObject.AddComponent<OwnerInformation>();
            //        oi.OwnerBody = summoner;
            //    }
            //});
        }
    }
}
