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
        internal static ConfigEntry<int> NoticeTime { get; set; }
        internal static ConfigEntry<int> Volume { get; set; }
        public static float ShieldTime => EntityStates.Engi.EngiBubbleShield.Deployed.lifetime - NoticeTime.Value - 1;

        internal EngiShieldProvider Provider { get; set; }

        public void Awake()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }

            const string sectionName = "BubbleShieldNotificationVolume";

            Volume = Config.AddSetting<int>(
                new ConfigDefinition(sectionName, nameof(Volume)),
                3,
                new ConfigDescription(
                    "0 = off, 1 = default, 2 = 2x default, etc... (up to 4x)  [recommended 3]",
                    new AcceptableValueRange<int>(0, 4)
                    ));

            NoticeTime = Config.AddSetting<int>(
                new ConfigDefinition(sectionName, nameof(NoticeTime)),
                3,
                new ConfigDescription(
                    "Time(s) for shield notification sounds to start playing",
                    new AcceptableValueRange<int>(1, 5)
                    ));

            Provider = new EngiShieldProvider();

            IL.EntityStates.Engi.EngiBubbleShield.Deployed.OnEnter += IL_Deployed_OnEnter;
            IL.EntityStates.Engi.EngiBubbleShield.Deployed.OnExit += IL_Deployed_OnExit;
        }

        public void Update()
        {
            Provider.Update(Time.deltaTime);
        }

        private void IL_Deployed_OnEnter(ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchPop()
                );
            c.Index -= 1;
            c.EmitDelegate<Func<GameObject, GameObject>>((gameObject) =>
            {
                Debug.Log("ONENETER");
                Provider.Add(gameObject);
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
                Debug.Log("ONEXIT");
                Provider.Remove(gameObject);
                return gameObject;
            });
        }
    }
}
