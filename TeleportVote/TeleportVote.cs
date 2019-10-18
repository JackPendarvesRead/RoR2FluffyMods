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
using FluffyLabsConfigManagerTools.Infrastructure;
using FluffyLabsConfigManagerTools.Util;

namespace TeleportVote
{
    [BepInPlugin("com.FluffyMods.TeleportVote", "TeleportVote", "2.0.0")]
    public class TeleportVote : BaseUnityPlugin
    {
        private VoteRegistrationController VoteController { get; set; }
        private TimerController TimerController { get; set; }

        public static ConfigEntry<bool> VotesEnabled;
        public static ConditionalConfigEntry<int> MaximumVotes;

        public void Awake()
        {
            VoteController = new VoteRegistrationController();
            TimerController = new TimerController();

            #region ConfigSetup
            const string votesSection = "Votes";

            VotesEnabled = Config.AddSetting<bool>(
                votesSection,
                "Enable Votes",
                true,
                new ConfigDescription(
                    "Enable/Disable voting"
                    ));

            var cUtil = new ConditionalUtil(this);
            MaximumVotes = cUtil.AddConditionalConfig<int>(
                votesSection,
                "Maximum Votes",
                4,
                false,
                new ConfigDescription(
                    "Enable to set maximum number of votes needed to continue (regardless of player count)."
                    ));
            #endregion

            #region HookRegistration
            //Main hooks - triggers restriction logics
            On.RoR2.TeleporterInteraction.OnInteractionBegin += TeleporterInteraction_OnInteractionBegin;
            On.RoR2.TeleporterInteraction.OnStateChanged += TeleporterInteraction_OnStateChanged;
            On.RoR2.Interactor.PerformInteraction += Interactor_PerformInteraction;
            On.RoR2.PlayerCharacterMasterController.OnBodyDeath += PlayerCharacterMasterController_OnBodyDeath;

            //Internal events
            //VoteController.PlayerRegistered += VoteController_PlayerRegistered;
            //TimerController.OnTeleporterChangeState += TimerController_OnTeleporterChangeState;

            //Chat Ready Command - type "r" to set yourself as ready
            Chat.onChatChanged += Chat_onChatChanged;

            //Prevent an exploitative interaction with teleporter and fireworks
            IL.RoR2.GlobalEventManager.OnInteractionBegin += GlobalEventManager_OnInteractionBegin;

            //Cleanup Hooks - Needed to avoid bugs where list persists from one run to another
            RoR2.Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
            On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.Run.EndStage += Run_EndStage;
            #endregion
        }

        #region ControllerTidyUp  
        private void StopAll()
        {
            VoteController.Reset();
            TimerController.Stop();
        }

        private void Run_onRunDestroyGlobal(Run obj)
        {
            StopAll();
        }

        private void Run_EndStage(On.RoR2.Run.orig_EndStage orig, Run self)
        {
            StopAll();
            orig(self);
        }

        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            StopAll();
            orig(self);
        }
        #endregion

        #region MainInteractionMethods
        private void TeleporterInteraction_OnInteractionBegin(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, TeleporterInteraction self, Interactor activator)
        {
            if (VotesEnabled.Value)
            {
                VoteController.RegisterPlayer(GetNetworkUserFromInteractor(activator));
                if (VoteController.VotesReady
                    || TimerController.TimerRestrictionsLifted)
                {
                    StopAll();
                    orig(self, activator);
                }
                else
                {
                    TimerController.Start();
                } 
            }
            else
            {
                orig(self, activator);
            }
        }

        private void Interactor_PerformInteraction(On.RoR2.Interactor.orig_PerformInteraction orig, Interactor self, GameObject interactableObject)
        {
            if (VotesEnabled.Value
                && InteractableObjectNames.IsRestictedInteractableObject(interactableObject.name))
            {
                VoteController.RegisterPlayer(GetNetworkUserFromInteractor(self));
                if (VoteController.VotesReady
                    || TimerController.TimerRestrictionsLifted)
                {
                    StopAll();
                    orig(self, interactableObject);
                }
                else
                {
                    TimerController.Start();
                }
            }
            else
            {
                orig(self, interactableObject);
            }
        }

        private void PlayerCharacterMasterController_OnBodyDeath(On.RoR2.PlayerCharacterMasterController.orig_OnBodyDeath orig, PlayerCharacterMasterController self)
        {
            if (VotesEnabled.Value)
            {
                Message.SendColoured("Player died. Reinstating restriction.", Colours.Red);
                StopAll();
            }           
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
                case 1: // IdleToCharging
                case 2: // Charging
                case 4: // Finished
                    VoteController.PlayersCanVote = false;
                    break;

                case 0: // Idle
                case 3: // Charged
                    VoteController.PlayersCanVote = true;
                    break;
            }
            orig(self, oldActivationState, newActivationState);
        }

        private NetworkUser GetNetworkUserFromInteractor(Interactor interactor)
        {
            return interactor.GetComponent<CharacterBody>().master.GetComponent<PlayerCharacterMasterController>().networkUser;
        }
        #endregion

        #region ChatCommand
        private static Regex ParseChatLog => new Regex(@"<color=#[0-9a-f]{6}><noparse>(?<name>.*?)</noparse>:\s<noparse>(?<message>.*?)</noparse></color>");
        private void Chat_onChatChanged()
        {
            try
            {

                if(VotesEnabled.Value
                    && VoteController.PlayersCanVote)
                {
                    var chatLog = Chat.readOnlyLog;
                    var match = ParseChatLog.Match(chatLog.Last());
                    var playerName = match.Groups["name"].Value.Trim();
                    var message = match.Groups["message"].Value.Trim();
                    Logger.LogDebug($"Chatlog={chatLog.Last()}, RMName={playerName}, RMMessage={message}");
                    if (!string.IsNullOrWhiteSpace(playerName))
                    {
                        switch (message.ToLower())
                        {
                            case "ready":
                            case "rdy":
                            case "r":
                            case "y":
                            case "go":
                                var netUser = RoR2.NetworkUser.readOnlyInstancesList
                                    .Where(x => x.userName.Trim() == playerName)
                                    .FirstOrDefault();
                                if (netUser.GetCurrentBody().healthComponent.alive)
                                {
                                    VoteController.RegisterPlayer(netUser);
                                }
                                break;
                        }
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
                x => x.MatchLdloc(2),               // Item Count
                x => x.MatchLdcI4(0),               // 0
                x => x.MatchBle(out ILLabel a));    // Transfers control to a target instruction if value is false, a null reference, or zero.

            c.Emit(OpCodes.Ldarg_2);
            c.EmitDelegate<Func<MonoBehaviour, bool>>((interactableThing) =>
            {
                if (interactableThing.name == InteractableObjectNames.Teleporter)
                {
                    return true;
                }
                return false;
            });
            c.Emit(OpCodes.Brtrue, returnLabel);
            c.GotoNext(x => x.MatchRet());
            c.MarkLabel(returnLabel);
        }
        #endregion       
    }
}
