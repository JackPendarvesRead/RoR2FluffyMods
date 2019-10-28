using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using FluffyLabsConfigManagerTools.Util;
using FluffyLabsConfigManagerTools.Infrastructure;

namespace PocketMoney
{
    [BepInDependency("com.FluffyMods.FluffyLabsConfigManagerTools")]
    [BepInPlugin("com.FluffyMods.PocketMoney", "PocketMoney", "2.0.0")]
    public class TestingStuff : BaseUnityPlugin
    {
        private ConditionalConfigEntry<uint> LatestStageToReceiveMoney;
        private ConfigEntry<uint> StageFlatMoney;
        private ConfigEntry<float> StageWeightedMoney;

        public void Awake()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }

            const string moneySection = "Money";

            var conUtil = new ConditionalUtil(this.Config);
            LatestStageToReceiveMoney = conUtil.AddConditionalConfig<uint>(
                moneySection,
                nameof(LatestStageToReceiveMoney),
                4,
                false,
                new ConfigDescription("Enable to set a latest stage you wish to receive a bonus on. E.g. set this to 4 and you will receive bonus for first 4 rounds and then none after.")
                );

            StageFlatMoney = Config.AddSetting<uint>(
                moneySection,
                nameof(StageFlatMoney),
                0,
                new ConfigDescription(
                    "The flat amount of extra money the player should receive at beginning of each stage"
                    )
                );

            StageWeightedMoney = Config.AddSetting<float>(
                moneySection,
                nameof(StageWeightedMoney),
                1.0f,
                new ConfigDescription(
                    "The equivalent number of small chest worth of money you get at start of each stage"
                    )
                );

            On.RoR2.Run.BeginStage += Run_BeginStage;
        }

        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            orig(self);

            if(LatestStageToReceiveMoney.Condition
                && LatestStageToReceiveMoney.Value > RoR2.Run.instance.stageClearCount)
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
