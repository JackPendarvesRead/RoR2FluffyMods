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
    [BepInPlugin("com.FluffyMods.PocketMoney", "PocketMoney", "1.1.1")]
    public class TestingStuff : BaseUnityPlugin
    {
        private static ConfigWrapper<uint> StageExtraMoney { get; set; }
        private static ConfigWrapper<uint> StageWeightedMoney { get; set; }

        public void Awake()
        {
            StageExtraMoney = Config.Wrap<uint>(
                "Money",
                "StageExtraMoney",
                "The flat amount of extra money the player should receive at beginning of each stage (uint)",
                0);

            StageWeightedMoney = Config.Wrap<uint>(
                "Money",
                "StageWeightedMoney",
                "The number of small chest worth of money you get at start of each stage (uint)",
                1);

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
            var difficultyScaledCost = (uint)Mathf.Round(RoR2.Run.instance.GetDifficultyScaledCost(25) * StageWeightedMoney.Value);
            var pocketMoney = StageExtraMoney.Value + difficultyScaledCost;
            foreach (var cm in RoR2.PlayerCharacterMasterController.instances)
            {
                cm.master.GiveMoney(pocketMoney);
            }
        }

        #region ConsoleCommands
        /// <summary>
        /// Sets the extra money given at start of each stage. args\[0\]=Value(int)
        /// </summary>
        /// <param name="args">args\[0\]=Value(int)</param>
        [ConCommand(commandName = "pocket_setflat", flags = ConVarFlags.ExecuteOnServer, helpText = "Sets the flat extra money. args[0]=Value(int).")]
        private static void PocketSetFlat(ConCommandArgs args)
        {
            try
            {
                StageExtraMoney.Value = (uint)Int32.Parse(args[0]);
                Debug.Log($"Flat money set to {args[0]}");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        /// <summary>
        /// Sets the extra money given at start of each stage. args\[0\]=Value(int)
        /// </summary>
        /// <param name="args">args\[0\]=Value(int)</param>
        [ConCommand(commandName = "pocket_setweighted", flags = ConVarFlags.ExecuteOnServer, helpText = "Sets the weighted extra money. args[0]=Value(int).")]
        private static void PocketSetWeighted(ConCommandArgs args)
        {
            try
            {
                StageWeightedMoney.Value = (uint)Int32.Parse(args[0]);
                Debug.Log($"Weighted money set to {args[0]}");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        #endregion

    }
}
