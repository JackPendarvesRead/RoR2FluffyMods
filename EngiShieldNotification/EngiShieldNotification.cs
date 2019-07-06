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

        public Timer ShieldTimer { get; set; }
        public Timer CountdownTimer { get; set; }
        private static string DestroySoundString;
        private static GameObject ShieldGameObject;

        public void Awake()
        {            
            ShieldTimer = new Timer(11 * 1000)
            {
                AutoReset = false,
                Enabled = false
            };
            ShieldTimer.Elapsed += ShieldTimer_Elapsed;

            countdownValue = 3;
            CountdownTimer = new Timer(1 * 1000)
            {
                AutoReset = true,
                Enabled = false
            };
            CountdownTimer.Elapsed += CountdownTimer_Elapsed;

            On.EntityStates.Engi.EngiBubbleShield.Deployed.OnEnter += Deployed_OnEnter;
            IL.EntityStates.Engi.EngiBubbleShield.Deployed.OnEnter += Deployed_OnEnter1;
            IL.EntityStates.Engi.EngiBubbleShield.Deployed.OnExit += Deployed_OnExit;            
        }        

        private void Deployed_OnEnter(On.EntityStates.Engi.EngiBubbleShield.Deployed.orig_OnEnter orig, EntityStates.Engi.EngiBubbleShield.Deployed self)
        {
            ResetTimers();
            ShieldTimer.Start();
            orig(self);
        }

        #region Timers
        private void ShieldTimer_Elapsed(object sender, ElapsedEventArgs e)
        {            
            CountdownTimer.Start();
        }

        private int countdownValue;
        private void CountdownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var num = Util.PlaySound(DestroySoundString, ShieldGameObject);
            --countdownValue;
            if (countdownValue <= 0)
            {
                ResetTimers();
            }
        }

        private void ResetTimers()
        {
            CountdownTimer.Stop();
            countdownValue = 5;
            ShieldTimer.Stop();
        }
        #endregion

        #region ILCode
        private void Deployed_OnEnter1(ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchPop()
                );
            c.Index -= 1;
            c.EmitDelegate<Func<GameObject, GameObject>>((gameObject) =>
            {
                ShieldGameObject = gameObject;
                return gameObject;
            });
        }

        private void Deployed_OnExit(ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdsfld(out FieldReference soundString),
                x => x.MatchLdarg(0)
                );
            c.Index += 1;
            c.EmitDelegate<Func<string, string>>((destroySoundString) =>
            {
                DestroySoundString = destroySoundString;
                return destroySoundString;
            });
        }
        #endregion

    }
}
