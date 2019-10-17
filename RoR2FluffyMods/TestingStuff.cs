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

namespace RoR2FluffyMods
{
    [BepInPlugin("com.FluffyMods.AAAAA", "AAAAA", "0.0.0")]
    public class TestingStuff : BaseUnityPlugin
    {
        public void Awake()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }
        }
    }
}
