using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace EngiShieldNotification
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.EngiShieldNotification", "EngiShieldNotification", "1.1.1")]
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

        private int NoticeTime => 3;
        internal static ConfigWrapper<int> VolumeConfig { get; set; }
        private int MaxVolume => 4;
        private int Volume
        {
            get
            {
                if (VolumeConfig.Value < 0) { return 0; }
                if (VolumeConfig.Value > MaxVolume) { return MaxVolume; }
                return VolumeConfig.Value;
            }
        }
        #endregion

        public void Awake()
        {
            On.RoR2.Console.Awake += (orig, self) =>
            {
                CommandHelper.RegisterCommands(self);
                orig(self);
            };

            VolumeConfig = Config.Wrap<int>(
                "BubbleShieldNotificationVolume",
                "Volume",
                "0 = off, 1 = default, 2 = 2x default, etc... (up to 4x)  [recommended 3]",
                3);

            On.EntityStates.Engi.EngiBubbleShield.Deployed.OnEnter += On_Deployed_OnEnter;
            IL.EntityStates.Engi.EngiBubbleShield.Deployed.OnEnter += IL_Deployed_OnEnter;
            On.EntityStates.Engi.EngiBubbleShield.Deployed.OnExit += On_Deployed_OnExit;
            IL.EntityStates.Engi.EngiBubbleShield.Deployed.OnExit += IL_Deployed_OnExit;
        }

        private void On_Deployed_OnEnter(On.EntityStates.Engi.EngiBubbleShield.Deployed.orig_OnEnter orig, EntityStates.Engi.EngiBubbleShield.Deployed self)
        {
            Task.Run(() => new EngiShieldNotificationController(this, EnterGameObject, (double)EntityStates.Engi.EngiBubbleShield.Deployed.lifetime, NoticeTime, Volume).Start());
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

        /// <summary>
        /// Set the volume of the Engineer bubble notification.
        /// </summary>
        /// <param name="args">Integer value 0-4 (0=mute, 4=loudest)</param>
        [ConCommand(commandName = "engishield_setvolume", flags = ConVarFlags.ExecuteOnServer, helpText = "Integer value 0-4 (0=mute, 4=loudest)")]
        private static void BoopGet(ConCommandArgs args)
        {
            try
            {
                if (args.Count != 1)
                {
                    throw new Exception("Command must take exactly one arguement.");
                }
                var value = Int32.Parse(args[0]);
                VolumeConfig.Value = value;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
