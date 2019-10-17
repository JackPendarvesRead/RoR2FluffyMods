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
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.FluffyLabsTest", "FluffyLabsTest", "0.0.0")]
    public class FluffyLabsTest : BaseUnityPlugin
    {
        private ConfigEntry<Macro> MyMacro1;
        private ConfigEntry<Macro> MyMacro2;
        private ConfigEntry<Macro> MyMacro3;
       
        public void Awake()
        {            
            const string macroSectionName = "Macro";
            var macroUtil = new MacroUtil(this);
            MyMacro1 = macroUtil.AddMacroConfig(macroSectionName, "macro 1", "description", false);
            MyMacro2 = macroUtil.AddMacroConfig(macroSectionName, "macro 2", "description", false);
            MyMacro3 = macroUtil.AddMacroConfig(macroSectionName, "macro 3", "description", true);
        }
    }   
}