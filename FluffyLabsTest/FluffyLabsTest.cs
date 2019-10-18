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
            var cUtil = new ConditionalUtil(this);
            ConditionalConf = cUtil.AddConditionalConfig<int>("con", "con", 5, true, new ConfigDescription("desc"));

            var bUtil = new ButtonUtil(this);
            bUtil.AddButtonConfig("Button", "button", "description", GetDic());

            var mUtil = new MacroUtil(this);
            MacroConf = mUtil.AddMacroConfig("Macro", "Macro", "Description", false);
        }
        
        private Dictionary<string, Action> GetDic()
        {
            return new Dictionary<string, Action>
            {
                { "button", () => { Debug.Log("Button action"); } }
            };
        }
    }   
}
