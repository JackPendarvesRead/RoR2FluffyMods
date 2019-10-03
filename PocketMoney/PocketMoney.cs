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
    [BepInPlugin("com.FluffyMods.PocketMoney", "PocketMoney", "2.0.0")]
    public class TestingStuff : BaseUnityPlugin
    {
        private static ConfigEntry<uint> StageFlatMoney { get; set; }
        private static ConfigEntry<uint> StageWeightedMoney { get; set; }
        private static ConfigEntry<uint> LatestStageToReceiveMoney { get; set; }

        public void Awake()
        {
            const string moneySection = "Money";

            LatestStageToReceiveMoney = Config.AddSetting<uint>(
                moneySection,
                nameof(LatestStageToReceiveMoney),
                0,
                new ConfigDescription(
                    "The latest stage you wish to receive a bonus on. Set to 0 to set no limit."
                    )
                );

            StageFlatMoney = Config.AddSetting<uint>(
                moneySection,
                nameof(StageFlatMoney),
                0,
                new ConfigDescription(
                    "The flat amount of extra money the player should receive at beginning of each stage (uint)"
                    )
                );

            StageWeightedMoney = Config.AddSetting<uint>(
                moneySection,
                nameof(StageWeightedMoney),
                1,
                new ConfigDescription(
                    "The number of small chest worth of money you get at start of each stage (uint)"
                    )
                );

            On.RoR2.Run.BeginStage += Run_BeginStage;
        }

        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            orig(self);

            if(LatestStageToReceiveMoney.Value > RoR2.Run.instance.stageClearCount)
            {
                return;
            }

            var difficultyScaledCost = (uint)Mathf.Round(RoR2.Run.instance.GetDifficultyScaledCost(25) * StageWeightedMoney.Value);
            var pocketMoney = StageFlatMoney.Value + difficultyScaledCost;
            foreach (var cm in RoR2.PlayerCharacterMasterController.instances)
            {
                cm.master.GiveMoney(pocketMoney);
            }
        }
    }
}
