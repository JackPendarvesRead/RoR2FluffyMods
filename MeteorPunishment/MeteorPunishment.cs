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
        private static CharacterBody PlayerToBePunished;
        private static bool CustomTarget = false;

        public void Awake()
        { 
            On.RoR2.EquipmentSlot.Execute += EquipmentSlot_Execute;
            IL.RoR2.MeteorStormController.MeteorWave.GetNextMeteor += MeteorWave_GetNextMeteor;
        }

        private void EquipmentSlot_Execute(On.RoR2.EquipmentSlot.orig_Execute orig, EquipmentSlot self)
        {
            if(self.equipmentIndex == EquipmentIndex.Meteor && !CustomTarget)
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
            c.EmitDelegate<Func<CharacterBody, CharacterBody>>((cb) =>
            {
                if(PlayerToBePunished != null && PlayerToBePunished.healthComponent.alive)
                {
                    return PlayerToBePunished;
                }
                else
                {
                    return null;
                }
            });        
        }


        /// <summary>
        /// Lists the targets for meteor punishment
        /// </summary>
        [ConCommand(commandName = "meteor_list", flags = ConVarFlags.ExecuteOnServer, helpText = "Lists the targets for meteor punishment")]
        private static void MeteorList(ConCommandArgs args)
        {
            var users = NetworkUser.readOnlyInstancesList;
            for(var i=0; i< users.Count; i++)
            {
                Debug.Log($"[{i}]: {users[i].userName}, Network_id={users[i].Network_id}");
            }
        }

        /// <summary>
        /// Set the target of meteor to a custom target
        /// </summary>
        [ConCommand(commandName = "meteor_set", flags = ConVarFlags.ExecuteOnServer, helpText = "args[0] = index of player to be punished")]
        private static void MeteorSet(ConCommandArgs args)
        {
            try
            {
                var playerIndex = Int32.Parse(args[0]);
                PlayerToBePunished = NetworkUser.readOnlyInstancesList[playerIndex].GetCurrentBody();
                CustomTarget = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// Sets meteor to hit whoever triggered it.
        /// </summary>
        [ConCommand(commandName = "meteor_default", flags = ConVarFlags.ExecuteOnServer, helpText = "Sets meteor to hit whoever triggered it.")]
        private static void MeteorDefault(ConCommandArgs args)
        {
            CustomTarget = false;
        }
    }
}