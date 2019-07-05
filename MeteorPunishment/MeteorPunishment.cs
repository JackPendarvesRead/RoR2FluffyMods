using BepInEx;
using BepInEx.Configuration;
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
        private CharacterBody PlayerToBePunished { get; set; }

        public void Awake()
        {
            On.RoR2.Console.Awake += (orig, self) =>
            {
                CommandHelper.RegisterCommands(self);
                orig(self);
            };

            On.RoR2.EquipmentSlot.Execute += EquipmentSlot_Execute;
            IL.RoR2.MeteorStormController.MeteorWave.GetNextMeteor += MeteorWave_GetNextMeteor;
        }

        private void EquipmentSlot_Execute(On.RoR2.EquipmentSlot.orig_Execute orig, EquipmentSlot self)
        {
            if(self.equipmentIndex == EquipmentIndex.Meteor)
            {
                PlayerToBePunished = self.characterBody;
            }
            orig(self);
        }

        private void MeteorWave_GetNextMeteor(MonoMod.Cil.ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdarg(0),
                x => x.MatchLdfld(out FieldReference fr1),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld(out FieldReference fr2),
                x => x.MatchLdelemRef()
                );
            c.GotoNext(x => x.MatchStloc(0));
            c.EmitDelegate<Func<CharacterBody, CharacterBody>>((charBody) =>
            {
                if(PlayerToBePunished != null && PlayerToBePunished.healthComponent.alive)
                {
                    return PlayerToBePunished;
                }
                else
                {
                    return charBody;
                }
            });
        }        
    }
}