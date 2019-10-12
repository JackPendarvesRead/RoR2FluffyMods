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
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.testing", "testing,my@STUPID!modNAME,.~#", "0.0.0")]
    public class Blerp : BaseUnityPlugin
    {
        const string modver = "0.0.0";

        public void Awake()
        {
            RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            RoR2.GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport obj)
        {
            throw new NotImplementedException();
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport obj)
        {
            throw new NotImplementedException();
        }
    }

    //[BepInDependency("com.bepis.r2api")]
    //[BepInPlugin("com.FluffyMods.AAAAA", "AAAAA", "0.0.0")]
    //public class TestingStuff : BaseUnityPlugin
    //{
    //    public void Awake()
    //    {
    //        IL.EntityStates.Huntress.ArrowRain.OnEnter += ArrowRain_OnEnter;
    //        On.EntityStates.Huntress.ArrowRain.OnEnter += ArrowRain_OnEnter1;
    //    }

    //    private void ArrowRain_OnEnter1(On.EntityStates.Huntress.ArrowRain.orig_OnEnter orig, EntityStates.Huntress.ArrowRain self)
    //    {
    //        Chat.AddMessage("AAAAA: This is the on message");
    //        orig(self);
    //    }

    //    private void ArrowRain_OnEnter(ILContext il)
    //    {
    //        Debug.Log("Mod AAAAA IL Method 1");
    //        var c = new ILCursor(il);
    //        c.EmitDelegate<Action>(() =>
    //        {
    //            Debug.Log("This is the delegate for plugin AAAAA");
    //            Chat.AddMessage("DELEGATE RAIN¬¬!!!");
    //        });
    //    }      
    //}

    //[BepInDependency("com.bepis.r2api")]
    //[BepInPlugin("com.FluffyMods.BBBBB", "BBBBB", "0.0.0")]
    //public class TestingStuff2 : BaseUnityPlugin
    //{

    //    public void Awake()
    //    {
    //        IL.EntityStates.Huntress.ArrowRain.OnEnter += ArrowRain_OnEnter;
    //        //On.EntityStates.Huntress.ArrowRain.OnEnter += ArrowRain_OnEnter1;
    //    }

    //    private void ArrowRain_OnEnter1(On.EntityStates.Huntress.ArrowRain.orig_OnEnter orig, EntityStates.Huntress.ArrowRain self)
    //    {
    //        Chat.AddMessage("This is the on message");
    //        orig(self);
    //    }

    //    private void ArrowRain_OnEnter(ILContext il)
    //    {
    //        Debug.Log("Mod BBBBB IL Method 1");
    //        var c = new ILCursor(il);
    //        c.EmitDelegate<Action>(() =>
    //        {
    //            Debug.Log("This is the delegate for plugin BBBB");
    //        });

    //    }
    //}
}
