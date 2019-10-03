using BepInEx;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace TeleportVote
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.TeleportVote", "TeleportVote", "2.0.0")]
    public class TeleportVote : BaseUnityPlugin
    {
        private RestrictionController Controller { get; set; }

        private ConfigEntry<bool> VotesEnabled { get; set; }
        private ConfigEntry<int> VotesNeeded { get; set; }
        private ConfigEntry<int> TimerNotificationInterval { get; set; }
        private ConfigEntry<int> TimerTimeUntilOverride { get; set; }

        public void Awake()
        {
            #region ConfigSetup
            const string votesSection = "Votes";
            const string timerSection = "Timer";

            VotesEnabled = Config.AddSetting<bool>(
                votesSection,
                nameof(VotesEnabled),
                true,
                new ConfigDescription(
                    "Enable/Disable voting"
                    ));

            VotesNeeded = Config.AddSetting<int>(
                votesSection,
                nameof(VotesEnabled),
                4,
                new ConfigDescription(
                    "Set maximum number of votes needed to continue (regardless of player count). Set to 0 for no limit.",
                    new AcceptableValueRange<int>(0, 16),
                    ConfigTags.Advanced
                    ));

            TimerNotificationInterval = Config.AddSetting<int>(
               timerSection,
               nameof(TimerNotificationInterval),
               15,
               new ConfigDescription(
                   "Time between chat notifications and reminders",
                   new AcceptableValueList<int>(15, 30),
                   ConfigTags.Advanced
                   ));

            TimerTimeUntilOverride = Config.AddSetting<int>(
               timerSection,
               nameof(TimerTimeUntilOverride),
               60,
               new ConfigDescription(
                   "Time after first vote until lock is overriden and you can use teleporter",
                   new AcceptableValueList<int>(30, 60, 90, 120),
                   ConfigTags.Advanced
                   ));
            #endregion

            #region HookRegistration
            //Main hooks - triggers restriction logics
            On.RoR2.TeleporterInteraction.OnInteractionBegin += TeleporterInteraction_OnInteractionBegin;
            On.RoR2.TeleporterInteraction.OnStateChanged += TeleporterInteraction_OnStateChanged;
            On.RoR2.Interactor.PerformInteraction += Interactor_PerformInteraction;
            On.RoR2.PlayerCharacterMasterController.OnBodyDeath += PlayerCharacterMasterController_OnBodyDeath;

            //Chat Ready Command - type "r" to set yourself as ready
            Chat.onChatChanged += Chat_onChatChanged;

            //Prevent an exploitative interaction with teleporter and fireworks
            IL.RoR2.GlobalEventManager.OnInteractionBegin += GlobalEventManager_OnInteractionBegin;

            //Cleanup Hooks - Needed to avoid bugs where list persists from one run to another
            On.RoR2.Run.BeginGameOver += Run_BeginGameOver;
            On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.Run.EndStage += Run_EndStage;
            #endregion

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
            Controller.TeleporterIsCharging = false;
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
            var userNetId = GetNetworkUserFromInteractor(activator);
            if (Controller.IsInteractionLegal(userNetId))
            {
                orig(self, activator);
            }
        }

        private void Interactor_PerformInteraction(On.RoR2.Interactor.orig_PerformInteraction orig, Interactor self, GameObject interactableObject)
        {            
            if (IsRestictableInteractableObject(interactableObject.name))
            {
                var userNetId = GetNetworkUserFromInteractor(self);
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

        private void PlayerCharacterMasterController_OnBodyDeath(On.RoR2.PlayerCharacterMasterController.orig_OnBodyDeath orig, PlayerCharacterMasterController self)
        {
            Controller.Stop();
            Message.SendToAll("Player died. Reinstating restriction.", Colours.Red);
            orig(self);
        }

        private void TeleporterInteraction_OnStateChanged(On.RoR2.TeleporterInteraction.orig_OnStateChanged orig, TeleporterInteraction self, int oldActivationState, int newActivationState)
        {
            //enum ActivationState           
            //Idle, 0            
            //IdleToCharging, 1                
            //Charging, 2        
            //Charged, 3            
            //Finished, 4

            switch (newActivationState)
            {
                case 1:
                case 2:
                    Controller.TeleporterIsCharging = true;
                    break;

                case 0:
                case 3:
                case 4:
                    Controller.TeleporterIsCharging = false;
                    break;
            }
            orig(self, oldActivationState, newActivationState);
        }

        /// <summary>
        /// Checks if an interactable should be checked for vote restriction logics
        /// </summary>
        /// <param name="interactableObjectName">String name of the interactable object</param>
        /// <returns>True if object should be run through TeleporterVote restriction logic</returns>
        private bool IsRestictableInteractableObject(string interactableObjectName)
        {
            foreach(var restrictedInteractable in InteractableObjectNames.GetAllRestrictedInteractableNames())
            {
                if(interactableObjectName.Trim().ToLower() == restrictedInteractable.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get the unique NetworkUserId of player. Credit to Wildbook for help in getting this.
        /// </summary>
        /// <param name="interactor">Interactor object belonging to the player</param>
        /// <returns>Unique NetworkUserId of player</returns>
        private NetworkUser GetNetworkUserFromInteractor(Interactor interactor)
        {
            return interactor.GetComponent<CharacterBody>().master.GetComponent<PlayerCharacterMasterController>().networkUser;
        }
        #endregion

        #region ChatReadyCommand
        private static Regex ParseChatLog => new Regex(@"<color=#[0-9a-f]{6}><noparse>(?<name>.*?)</noparse>:\s<noparse>(?<message>.*?)</noparse></color>");
        private void Chat_onChatChanged()
        {
            try
            {
                var chatLog = Chat.readOnlyLog;
                var match = ParseChatLog.Match(chatLog.Last());
                var name = match.Groups["name"].Value.Trim();
                var message = match.Groups["message"].Value.Trim();
                Logger.LogDebug($"Chatlog={chatLog.Last()}, RMName={name}, RMMessage={message}");
                if (!string.IsNullOrWhiteSpace(name))
                {
                    switch (message.ToLower())
                    {
                        case "ready":
                        case "rdy":
                        case "r":
                        case "y":
                        case "go":
                            var netUser = (from u in RoR2.NetworkUser.readOnlyInstancesList
                                           where u.userName.Trim() == name
                                           select u).FirstOrDefault();
                            if (netUser.GetCurrentBody().healthComponent.alive)
                            {
                                Controller.ChatCommandReady(netUser);
                            }
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
            ILLabel returnLabel = il.DefineLabel();
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
            c.Emit(OpCodes.Brtrue, returnLabel); 
            c.GotoNext(x => x.MatchRet());
            c.MarkLabel(returnLabel);
        }
        #endregion       
    }
}
