using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace EngiShieldNotification
{
    [BepInPlugin("com.FluffyMods.EngiShieldNotification", "EngiShieldNotification", "2.0.0")]
    public class EngiShieldNotification : BaseUnityPlugin
    {
        #region FieldsEtc
        internal class OnDestroyExitGameObjectEventArgs : EventArgs
        {
            public OnDestroyExitGameObjectEventArgs(GameObject exitGameObject)
            {
                ExitGameObject = exitGameObject;
            }

            public GameObject ExitGameObject { get; }
        }

        public event EventHandler OnDestroyExitGameObject;
        private static GameObject EnterGameObject { get; set; }
        private static GameObject ExitGameObject { get; set; }

        private int NoticeTime = 3;
        internal static ConfigEntry<int> Volume { get; set; }        
        #endregion

        public void Awake()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }

            Volume = Config.AddSetting<int>(
                new ConfigDefinition("BubbleShieldNotificationVolume", nameof(Volume)),
                3,
                new ConfigDescription(
                    "0 = off, 1 = default, 2 = 2x default, etc... (up to 4x)  [recommended 3]",
                    new AcceptableValueRange<int>(0, 4)
                    )
                );

            IL.EntityStates.Engi.EngiBubbleShield.Deployed.OnEnter += IL_Deployed_OnEnter;
            IL.EntityStates.Engi.EngiBubbleShield.Deployed.OnExit += IL_Deployed_OnExit;
            On.EntityStates.Engi.EngiBubbleShield.Deployed.OnEnter += On_Deployed_OnEnter;
            On.EntityStates.Engi.EngiBubbleShield.Deployed.OnExit += On_Deployed_OnExit;
        }

        private void On_Deployed_OnEnter(On.EntityStates.Engi.EngiBubbleShield.Deployed.orig_OnEnter orig, EntityStates.Engi.EngiBubbleShield.Deployed self)
        {
            Task.Run(() =>            
            new EngiShieldNotificationController(
                this, 
                EnterGameObject, 
                EntityStates.Engi.EngiBubbleShield.Deployed.lifetime, 
                NoticeTime, 
                Volume.Value
                ).Start());
            orig(self);
        }

        private void On_Deployed_OnExit(On.EntityStates.Engi.EngiBubbleShield.Deployed.orig_OnExit orig, EntityStates.Engi.EngiBubbleShield.Deployed self)
        {
            OnDestroyExitGameObject.Invoke(this, new OnDestroyExitGameObjectEventArgs(ExitGameObject));
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
                EnterGameObject = gameObject;
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
                ExitGameObject = gameObject;
                return gameObject;
            });
        }
        #endregion        
    }
}
