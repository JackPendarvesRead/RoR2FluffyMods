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

namespace ShapedGlassBlackScreenFix
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.ZZZZZ01", "ZZZZZ01", "0.0.0")]
    public class TestingStuff : BaseUnityPlugin
    {
        public void Awake()
        {
            IL.EntityStates.Huntress.ArrowRain.OnEnter += ArrowRain_OnEnter;
            IL.EntityStates.Huntress.ArrowRain.OnEnter += ArrowRain_OnEnter2;
        }
       
        private void ArrowRain_OnEnter(ILContext il)
        {
            Debug.Log("THIS IS METHOD 1");
            var c = new ILCursor(il);
            c.EmitDelegate<Action>(() =>
            {
                Chat.AddMessage("This is method 01");
            });
            Debug.Log(c);
            c.Emit(OpCodes.Ldc_I4_2);
            Debug.Log(c);
        }

        private void ArrowRain_OnEnter2(ILContext il)
        {
            Debug.Log("THIS IS METHOD 3");
            var c = new ILCursor(il);
            c.EmitDelegate<Action>(() =>
            {
                Chat.AddMessage("This is method 03");
            });
            Debug.Log(c);
            c.Emit(OpCodes.Ldc_I4_2);
            Debug.Log(c);
        }
    }


    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.ZZZZZ02", "ZZZZZ02", "0.0.0")]
    public class TestingStuff2 : BaseUnityPlugin
    {
        public void Awake()
        {
            IL.EntityStates.Huntress.ArrowRain.OnEnter += ArrowRain_OnEnter;
        }

        private void ArrowRain_OnEnter(ILContext il)
        {
            Debug.Log("THIS IS METHOD 2");
            var c = new ILCursor(il);
            c.EmitDelegate<Action>(() =>
            {
                Chat.AddMessage("This is method 02");
            });
            Debug.Log(c);
            c.Emit(OpCodes.Ldc_I4_2);
            Debug.Log(c);
        }
    }
}
