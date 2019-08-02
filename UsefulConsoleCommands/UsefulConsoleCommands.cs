using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using R2API;
using R2API.Utils;

namespace UsefulConsoleCommands
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.UsefulConsoleCommands", "UsefulConsoleCommands", "0.0.0")]
    public class TestingStuff : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.Console.Awake += (orig, self) =>
            {
                CommandHelper.RegisterCommands(self);
                orig(self);
            };
        }

        [ConCommand(commandName = "coin_give", flags = ConVarFlags.ExecuteOnServer, helpText = "Give lunar coins. args[0]=(int)value")]
        private static void CoinGive(ConCommandArgs args)
        {
            foreach (var nu in NetworkUser.readOnlyInstancesList)
            {
                var before = nu.NetworknetLunarCoins;
                var num = (uint)Int32.Parse(args[0]);
                nu.AwardLunarCoins(num);
                var after = nu.NetworknetLunarCoins;
                Debug.Log($"B={before}, A={after}");
            }
        }

        [ConCommand(commandName = "coin_take", flags = ConVarFlags.ExecuteOnServer, helpText = "Deduct lunar coins. args[0]=(int)value")]
        private static void CoinTake(ConCommandArgs args)
        {
            foreach (var nu in NetworkUser.readOnlyInstancesList)
            {
                var before = nu.NetworknetLunarCoins;
                var num = (uint)Int32.Parse(args[0]);
                nu.DeductLunarCoins(num);
                var after = nu.NetworknetLunarCoins;
                Debug.Log($"B={before}, A={after}");
            }
        }
    }
}
