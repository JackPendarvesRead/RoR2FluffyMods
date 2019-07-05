using BepInEx;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Linq;
using UnityEngine;

namespace MeteorPunishment
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.MeteorPunishment", "MeteorPunishment", "1.0.1")]
    public class MeteorPunishment : BaseUnityPlugin
    {
        public void Awake()
        {           
            IL.RoR2.MeteorStormController.MeteorWave.GetNextMeteor += MeteorWave_GetNextMeteor;
        }

        private void MeteorWave_GetNextMeteor(MonoMod.Cil.ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdarg(0),                               //this
                x => x.MatchLdfld(out FieldReference fr1),          // class RoR2.CharacterBody[] RoR2.MeteorStormController/MeteorWave::targets
                x => x.MatchLdarg(0),                               //this
                x => x.MatchLdfld(out FieldReference fr2),          //int32 RoR2.MeteorStormController/MeteorWave::currentStep
                x => x.MatchLdelemRef()                             //
                );
            c.GotoNext(x => x.MatchStloc(0));
            c.EmitDelegate<Func<CharacterBody, CharacterBody>>((charBody) =>
            {
                var punishBody = (from n in RoR2.NetworkUser.readOnlyInstancesList
                                      //where n.userName.Trim().ToLower() == PlayerToPunish.Value.Trim().ToLower()
                                      select n.GetCurrentBody()).FirstOrDefault();
                return punishBody;
            });
        }
    }
}