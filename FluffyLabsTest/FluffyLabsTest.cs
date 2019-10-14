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

namespace FluffyLabsTest
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.FluffyLabsTest", "FluffyLabsTest", "0.0.0")]
    public class FluffyLabsTest : BaseUnityPlugin
    {
        public void Awake()
        {
        }

       
    }   
}
