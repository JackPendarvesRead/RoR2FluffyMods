using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace PocketMoney
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.PocketMoney", "PocketMoney", "1.0.0")]
    public class TestingStuff : BaseUnityPlugin
    {
        private static ConfigWrapper<uint> StartMoney { get; set; }

        public void Awake()
        {
            StartMoney = Config.Wrap<uint>(
                "Money",
                "RoundExtraMoney",
                "The amount of money the player should receive at beginning of each stage.",
                50);

            On.RoR2.Console.Awake += (orig, self) =>
            {
                CommandHelper.RegisterCommands(self);
                orig(self);
            };

            On.RoR2.Run.BeginStage += Run_BeginStage;
        }

        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            orig(self);
            foreach (var cm in RoR2.PlayerCharacterMasterController.instances)
            {
                cm.master.GiveMoney(StartMoney.Value);
            }
        }

        /// <summary>
        /// Sets the extra money given at start of each round. args\[0\]=Value(int)
        /// </summary>
        /// <param name="args">args\[0\]=Value(int)</param>
        [ConCommand(commandName = "pocket_set", flags = ConVarFlags.ExecuteOnServer, helpText = "Sets the extra money given at start of each round. args\[0\]=Value(int).")]
        private static void PocketSet(ConCommandArgs args)
        {
            try
            {
                StartMoney.Value = (uint)Int32.Parse(args[0]);
                Debug.Log($"Round start money set to {args[0]}");
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }
}
