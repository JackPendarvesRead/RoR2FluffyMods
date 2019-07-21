using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace RoR2FluffyMods
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.RoR2FluffyMods", "RoR2FluffyMods", "0.0.0")]
    public class TestingStuff : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.Run.Start += Run_Start;
        }

        private void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);
            var s = "prefabs/projectiles/engibubbleshield";
            var x = Resources.Load(s);

        }
    }
}
