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
        private ConditionalConfigEntry<int> ConditionalConf;
        private MacroConfigEntry MacroConf;

        public void Awake()
        {
            Config.AddSetting<string>("", "", "test");

            var cUtil = new ConditionalUtil(this.Config);
            ConditionalConf = cUtil.AddConditionalConfig<int>("con", "con", 5, true, new ConfigDescription("desc"));

            var bUtil = new ButtonUtil(this.Config);
            bUtil.AddButtonConfig("Button", "Button 1", "description", GetDic(), false);
            bUtil.AddButtonConfig("Button", "Button 2", "description", GetDic(), true);

            var mUtil = new MacroUtil(this.Config);
            MacroConf = mUtil.AddMacroConfig("Macro", "Macro", "Description");
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
