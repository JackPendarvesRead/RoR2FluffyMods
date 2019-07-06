using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;

namespace EngiShieldNotification
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.EngiShieldNotification", "EngiShieldNotification", "1.0.1")]
    public class EngiShieldNotification : BaseUnityPlugin
    {
        private static GameObject OnEnterGameObject { get; set; }
        private static GameObject OnExitGameObject { get; set; }        

        public void Awake()
        {    
            On.EntityStates.Engi.EngiBubbleShield.Deployed.OnEnter += On_Deployed_OnEnter;
            IL.EntityStates.Engi.EngiBubbleShield.Deployed.OnEnter += IL_Deployed_OnEnter;
            On.EntityStates.Engi.EngiBubbleShield.Deployed.OnExit += On_Deployed_OnExit;
            IL.EntityStates.Engi.EngiBubbleShield.Deployed.OnExit += IL_Deployed_OnExit;            
        }       

        private void On_Deployed_OnEnter(On.EntityStates.Engi.EngiBubbleShield.Deployed.orig_OnEnter orig, EntityStates.Engi.EngiBubbleShield.Deployed self)
        {
            Task.Run(() => new EngiShieldNotificationController(OnEnterGameObject).Start());
            orig(self);
        }

        private void On_Deployed_OnExit(On.EntityStates.Engi.EngiBubbleShield.Deployed.orig_OnExit orig, EntityStates.Engi.EngiBubbleShield.Deployed self)
        {
            orig(self);
        }

        #region ILCode
        private void IL_Deployed_OnEnter(ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchPop()
                );
            c.Index -= 1;
            c.EmitDelegate<Func<GameObject, GameObject>>((gameObject) =>
            {
                OnEnterGameObject = gameObject;
                return gameObject;
            });
        }

        private void IL_Deployed_OnExit(ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchPop()
                );
            c.Index -= 1;
            c.EmitDelegate<Func<GameObject, GameObject>>((gameObject) =>
            {
                OnExitGameObject = gameObject;
                return gameObject;
            });
        }
        #endregion

    }
}
