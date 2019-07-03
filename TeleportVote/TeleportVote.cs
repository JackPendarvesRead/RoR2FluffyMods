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
using UnityEngine.Networking;

namespace TeleportVote
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.TeleportVote", "TeleportVote", "1.0.1")]
    public class TeleportVote : BaseUnityPlugin
    {   
        private RestrictionController Controller { get; set; }

        public void Awake()
        {
            int notificationInterval = 15;
            int timeUntilUnlock = 60;
            this.Controller = new RestrictionController(notificationInterval, timeUntilUnlock);            

            //TODO ping teleporter
            //TODO add config???
            
            //Main hooks - triggers restriction logics
            On.RoR2.TeleporterInteraction.OnInteractionBegin += TeleporterInteraction_OnInteractionBegin;
            On.RoR2.Interactor.PerformInteraction += Interactor_PerformInteraction;

            //Chat Ready Command - type "r" to set yourself as ready
            Chat.onChatChanged += Chat_onChatChanged;

            //Prevent an exploitative interaction with teleporter and fireworks
            IL.RoR2.GlobalEventManager.OnInteractionBegin += GlobalEventManager_OnInteractionBegin;
            
            //Cleanup Hooks - Needed to avoid bugs where list persists from one run to another
            On.RoR2.Run.BeginGameOver += Run_BeginGameOver;
            On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.Run.EndStage += Run_EndStage;
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

        #region MainHookMethods
        private void TeleporterInteraction_OnInteractionBegin(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, TeleporterInteraction self, Interactor activator)
        {
            var userNetId = GetNetworkIdFromInteractor(activator);
            Logger.LogInfo($"TPInt: userNetId={userNetId}, userNetId.Value={userNetId.value}");
            if (Controller.IsInteractionLegal(userNetId))
            {
                orig(self, activator);
            }
        }

        private void Interactor_PerformInteraction(On.RoR2.Interactor.orig_PerformInteraction orig, Interactor self, GameObject interactableObject)
        {            
            if (IsRestictableInteractableObject(interactableObject.name))
            {
                var userNetId = GetNetworkIdFromInteractor(self);
                Logger.LogInfo($"IntPerform: userNetId={userNetId}");
                if (Controller.IsInteractionLegal(userNetId))
                {
                    orig(self, interactableObject);
                }
            }
            else
            {
                orig(self, interactableObject);
            }
        }

        /// <summary>
        /// Checks if an interactable should be checked for vote restriction logics
        /// </summary>
        /// <param name="interactableObjectName">String name of the interactable object</param>
        /// <returns>True if object should be run through TeleporterVote restriction logic</returns>
        private bool IsRestictableInteractableObject(string interactableObjectName)
        {
            if (interactableObjectName == InteractableObjectNames.PortalShopClone || interactableObjectName == InteractableObjectNames.PortalShop)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get the unique NetworkUserId of player. Credit to Wildbook for help in getting this.
        /// </summary>
        /// <param name="interactor">Interactor object belonging to the player</param>
        /// <returns>Unique NetworkUserId of player</returns>
        private NetworkUserId GetNetworkIdFromInteractor(Interactor interactor)
        {
            var netId = interactor.GetComponent<CharacterBody>().master.GetComponent<PlayerCharacterMasterController>().networkUser.Network_id;
            Logger.LogInfo($"FromInteractor: netId={netId}");
            return netId;
        }
        #endregion

        #region ChatReadyCommand
        private static Regex ParseChatLog => new Regex(@"<color=#[0-9a-f]{6}><noparse>(?<name>.*?)</noparse>:\s<noparse>(?<message>.*?)</noparse></color>");
        private void Chat_onChatChanged()
        {
            try
            {
                var networkUsers = RoR2.NetworkUser.readOnlyInstancesList;
                foreach (var user in networkUsers)
                {
                    Logger.LogInfo($"ALLNETUSERS: NetworkUserName={user.name}, NetworkUserUsername={user.userName}, NetworkUserNetId={user.netId.Value}, Network_id={user.Network_id.value}");
                }

                var chatLog = Chat.readOnlyLog;
                var m = ParseChatLog.Match(chatLog.Last());
                var name = m.Groups["name"].Value;
                var message = m.Groups["message"].Value;

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var netUser = (from u in networkUsers
                                   where u.userName.Trim() == name.Trim()
                                   select u.Network_id).FirstOrDefault();
                    Logger.LogInfo($"LastChatLog: {chatLog.Last()}");
                    Logger.LogInfo($"Regex: name={name}, message={message}.  netId={netUser}");
                    switch (message.ToLower())
                    {
                        case "ready":
                        case "rdy":
                        case "r":
                        case "y":
                        case "go":
                            Controller.AddChatReady(netUser);
                            break;
                    }
                }

                
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        #endregion

        #region FireWorksILDisable
        /// <summary>
        /// Prevent fireworks from triggering when interacting with teleporter or portals
        /// </summary>
        /// <param name="il">il context</param>
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

            //Credit to paddywan for guidance in formulating this IL hook
        }
        #endregion       
    }
}
