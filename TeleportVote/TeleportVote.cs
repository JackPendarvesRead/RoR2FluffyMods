using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
using System.Text.RegularExpressions;

namespace TeleportVote
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.TeleportVote", "TeleportVote", "1.0.1")]
    public class TeleportVote : BaseUnityPlugin
    {   
        private RestrictionController Controller { get; set; }

        public void Awake()
        {
            int notificationInterval = 30;
            int timeUntilUnlock = 90;
            this.Controller = new RestrictionController(notificationInterval, timeUntilUnlock);

            //TODO chat command R for ready
            Chat.onChatChanged += Chat_onChatChanged;

            //TODO ping teleporter

            #region ConfigWrapperSetup

            //TODO add ConfigWrapper for time to spend waiting for all players to be ready
            //TODO add ConfigWrapper for maxplayers?
            //TODO add ConfigWrapper for TimeLimit

            #endregion
            
            //Main hooks
            On.RoR2.TeleporterInteraction.OnInteractionBegin += TeleporterInteraction_OnInteractionBegin;
            On.RoR2.Interactor.PerformInteraction += Interactor_PerformInteraction;

            //Fire Fireworks exploitative interaction with teleporter
            IL.RoR2.GlobalEventManager.OnInteractionBegin += GlobalEventManager_OnInteractionBegin;

            //Cleanup Hooks
            On.RoR2.Run.BeginGameOver += Run_BeginGameOver;
            On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.Run.EndStage += Run_EndStage;
        }

        private static Regex ParseChatLog => new Regex("<color=#e5eefc><noparse>(?<name>.*?)</noparse>: <noparse>(?<message>.*?)</noparse></color>");
        private void Chat_onChatChanged()
        {
            try
            {
                foreach (var user in RoR2.NetworkUser.readOnlyInstancesList)
                {
                    Logger.LogInfo($"NetworkUserName={user.name}, NetworkUserUsername={user.userName}, NetworkUserNetId={user.netId}");
                }
            }
            catch
            {
                Logger.LogInfo("UnableToGetNetworkUsers!");
            }
            

            var chatLog = Chat.readOnlyLog;
            var m = ParseChatLog.Match(chatLog.Last());
            var name = m.Groups["name"].Value;
            var message = m.Groups["message"].Value;
            Logger.LogInfo($"Regex: name={name}, message={message}");

            switch (message.ToLower())
            {
                case "ready":
                case "rdy":
                case "r":
                case "y":
                case "go":
                    Controller.AddChatReady(name);
                    break;
            }
        }

        #region ControllerTidyUp   

        private void Run_EndStage(On.RoR2.Run.orig_EndStage orig, Run self)
        {
            Controller.Stop();
            orig(self);
        }

        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            Controller.Stop();
            orig(self);
        }

        private void Run_BeginGameOver(On.RoR2.Run.orig_BeginGameOver orig, Run self, GameResultType gameResultType)
        {
            if (self.isGameOverServer)
            {
                Controller.Stop();
            }
            orig(self, gameResultType);
        }

        #endregion


        private void TeleporterInteraction_OnInteractionBegin(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, TeleporterInteraction self, Interactor activator)
        {
            var playerId = activator.netId.ToString();
            Logger.LogInfo($"TeleInt: playernetId={playerId}");
            if (Controller.IsInteractionLegal(playerId))
            {
                orig(self, activator);
            }
        }

        private void Interactor_PerformInteraction(On.RoR2.Interactor.orig_PerformInteraction orig, Interactor self, GameObject interactableObject)
        {
            if (IsRestictableInteractableObject(interactableObject.name))
            {
                var playerId = self.netId.ToString();
                Logger.LogInfo($"InterPerf: playernetId={playerId}");
                if (Controller.IsInteractionLegal(playerId))
                {
                    orig(self, interactableObject);
                }
            }
            else
            {
                orig(self, interactableObject);
            }
        }

        private void GlobalEventManager_OnInteractionBegin(ILContext il)
        {
            ILLabel myLabel = il.DefineLabel();
            var c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdloc(2),                                       // Item Count
                x => x.MatchLdcI4(0),                                       // 0
                x => x.MatchBle(out ILLabel a),                             // <=
                x => x.MatchLdarg(2),                                       // interactable
                x => x.MatchCastclass(out TypeReference typeReference),     // Monobehaviour type
                x => x.MatchCall(out MethodReference methodReference),      // FireworksLogic true/false logic
                x => x.MatchBrfalse(out ILLabel b));                        // Transfers control to a target instruction if value is false, a null reference, or zero.

            c.Emit(OpCodes.Ldarg_2);
            c.EmitDelegate<Func<MonoBehaviour, bool>>((interactableThing) =>
            {
                if (interactableThing.name == InteractableObjectNames.Teleporter) { return true; }
                else { return false; }
            });
            c.Emit(OpCodes.Brtrue, myLabel);

            //Go to return
            c.GotoNext(x => x.MatchRet());
            c.MarkLabel(myLabel);
        }              

        private bool IsRestictableInteractableObject(string interactableObjectName)
        {
            if (interactableObjectName == InteractableObjectNames.Shop || interactableObjectName == InteractableObjectNames.Shop2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
