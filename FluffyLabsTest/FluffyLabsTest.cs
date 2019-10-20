using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using MonoMod.Cil;
using UnityEngine;
using Mono.Cecil.Cil;
using BepInEx.Configuration;
using FluffyLabsConfigManagerTools.Util;
using FluffyLabsConfigManagerTools.Infrastructure;

namespace FluffyLabsTest
{
    [BepInPlugin("com.FluffyMods.FluffyLabsTest", "FluffyLabsTest", "0.0.0")]
    public class FluffyLabsTest : BaseUnityPlugin
    {
        private ConditionalConfigEntry<float> ConditionalConf;
        private ConfigEntry<float> floatCon;
        private ConfigEntry<double> dubCon;
        private ConfigEntry<int> intCon;

        public void Awake()
        {
            const string testSection = "TESTTEST";
            floatCon = Config.AddSetting<float>(testSection, "float", 5f, new ConfigDescription("", new AcceptableValueRange<float>(0,20)));
            dubCon = Config.AddSetting<double>(testSection, "double", 8.0, new ConfigDescription("", new AcceptableValueRange<double>(0, 20)));
            intCon = Config.AddSetting<int>(testSection, "int", 2, new ConfigDescription("", new AcceptableValueRange<int>(0, 20)));

            var cUtil = new ConditionalUtil(this.Config);
            ConditionalConf = cUtil.AddConditionalConfig<float>("con", "con", 5f, true, new ConfigDescription("desc"));

        }
        
        private Dictionary<string, Action> GetDic()
        {
            return new Dictionary<string, Action>
            {
                { "button 1", () => { Debug.Log("Button action 1"); } },
                { "button 2", () => { Debug.Log("Button action 2"); } },
                { "button 3", () => { Debug.Log("Button action 3"); } }
            };
        }
    }   
}
