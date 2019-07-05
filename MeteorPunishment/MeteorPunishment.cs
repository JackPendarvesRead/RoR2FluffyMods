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
        private static NetworkUser PlayerToBePunished { get; set; }

        public void Awake()
        {
            
            //PlayerToBePunished = Config.Wrap<string>("PlayerToBePunished", "PlayerToBePunished", "The Steam username of the player to be punished.", null);

            On.RoR2.Console.Awake += (orig, self) =>
            {
                CommandHelper.RegisterCommands(self);
                orig(self);
            };

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
                var punishBody = PlayerToBePunished.GetCurrentBody();
                if(punishBody != null && punishBody.healthComponent.alive)
                {
                    return punishBody;
                }
                else
                {
                    return charBody;
                }
            });
        }

        /// <summary>
        /// Set player to be punished. Use NetworkUser index as arguement.
        /// </summary>
        [ConCommand(commandName = "punish_set", flags = ConVarFlags.ExecuteOnServer, helpText = "args[0]=index of player to be punished.")]
        private static void SetPunishPlayer(ConCommandArgs args)
        {
            try
            {
                if (args.Count != 1)
                {
                    throw new Exception("Command must take 1 arguement.");
                }
                var punishIndex = Int32.Parse(args[0]);
                var netUsers = RoR2.NetworkUser.readOnlyInstancesList;
                PlayerToBePunished = netUsers[punishIndex];
                Debug.Log($"[{punishIndex}]{netUsers[punishIndex].userName} has been set as punished player.");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        /// <summary>
        /// Lists players which are available for punishment.
        /// </summary>
        [ConCommand(commandName = "punish_list", flags = ConVarFlags.ExecuteOnServer, helpText = "Lists players which are available for punishment.")]
        private static void ListPunishPlayer(ConCommandArgs args)
        {
            var netUsers = RoR2.NetworkUser.readOnlyInstancesList;
            for (var i = 0; i < netUsers.Count; i++)
            {
                Debug.Log($"[{i}]: {netUsers[i].userName}, {netUsers[i].Network_id.value}");
            }
        }
        /// <summary>
        /// Sets player to be punished to null.
        /// </summary>
        [ConCommand(commandName = "punish_clear", flags = ConVarFlags.ExecuteOnServer, helpText = "Sets player to be punished to null.")]
        private static void ClearPunishPlayer(ConCommandArgs args)
        {
            PlayerToBePunished = null;
        }
    }
}