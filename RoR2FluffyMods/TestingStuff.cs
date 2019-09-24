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

namespace ShapedGlassBlackScreenFix
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.ZZZZZ01", "ZZZZZ01", "0.0.0")]
    public class TestingStuff : BaseUnityPlugin
    {
        ConfigWrapper<bool> MySetting { get; set; }

        public void Awake()
        {
        }       
        
    }
}
