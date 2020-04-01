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
using System.Reflection;
using BepInEx.Bootstrap;

namespace PocketMoney
{
    [BepInDependency(FluffyLabsConfigManagerTools.FluffyConfigLabsPlugin.PluginGuid)]
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    [BepInDependency("com.funkfrog_sipondo.sharesuite", BepInDependency.DependencyFlags.SoftDependency)]
    public class TestingStuff : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "PocketMoney";
        private const string pluginVersion = "2.1.0";

        private ConditionalConfigEntry<uint> LatestStageToReceiveMoney;
        private ConfigEntry<uint> StageFlatMoney;
        private ConfigEntry<float> StageWeightedMoney;

        private MethodInfo AddMoney = null;
        private BaseUnityPlugin ShareSuite = null;

        public void Awake()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }
                        
            if (Chainloader.PluginInfos.ContainsKey("com.funkfrog_sipondo.sharesuite"))
            {
                ShareSuite = Chainloader.PluginInfos["com.funkfrog_sipondo.sharesuite"].Instance;
                AddMoney = ShareSuite.GetType().GetMethod("AddMoneyExternal", BindingFlags.Instance | BindingFlags.Public);
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

            StageFlatMoney = Config.Bind<uint>(
                moneySection,
                nameof(StageFlatMoney),
                0,
                new ConfigDescription(
                    "The flat amount of extra money the player should receive at beginning of each stage"
                    )
                );

            StageWeightedMoney = Config.Bind<float>(
                moneySection,
                nameof(StageWeightedMoney),
                1.0f,
                new ConfigDescription(
                    "The equivalent number of small chest worth of money you get at start of each stage"
                    )
                );

            RoR2.SceneDirector.onPostPopulateSceneServer += SceneDirector_onPostPopulateSceneServer;
        }

        private void SceneDirector_onPostPopulateSceneServer(SceneDirector obj)
        {
            if (RoR2.Run.instance)
            {
                if (!LatestStageToReceiveMoney.Condition
                    && LatestStageToReceiveMoney.Value <= RoR2.Run.instance.stageClearCount)
                {
                    var difficultyScaledCost = (uint)Mathf.Round(RoR2.Run.instance.GetDifficultyScaledCost(25) * StageWeightedMoney.Value);
                    var pocketMoney = StageFlatMoney.Value + difficultyScaledCost;

                    if (ShareSuite)
                    {
                        AddMoney.Invoke(ShareSuite, new object[] { pocketMoney });
                    }
                    else
                    {
                        foreach (var cm in RoR2.PlayerCharacterMasterController.instances)
                        {
                            cm.master.GiveMoney(pocketMoney);
                        };
                    }                    
                }
            }
        }
    }
}
