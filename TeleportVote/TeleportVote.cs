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
        private VoteRegistrationController VoteController { get; set; } = new VoteRegistrationController();
        private TimerController TimerController { get; set; } = new TimerController();
        
        public static ConfigEntry<bool> VotesEnabled { get; set; }
        public static ConfigEntry<int> MaximumVotes { get; set; }
        //public static ConfigEntry<int> TimerNotificationInterval { get; set; }
        //public static ConfigEntry<int> TimeUntilVoteOverride { get; set; }

        public void Awake()
        {            
            #region ConfigSetup
            const string votesSection = "Votes";
            //const string timerSection = "Timer";

            VotesEnabled = Config.AddSetting<bool>(
                votesSection,
                "Enable Votes",
                true,
                new ConfigDescription(
                    "Enable/Disable voting"
                    ));

            MaximumVotes = Config.AddSetting<int>(
                votesSection,
                "Maximum Votes",
                4,
                new ConfigDescription(
                    "Set maximum number of votes needed to continue (regardless of player count). Set to 0 for no limit.",
                    new AcceptableValueRange<int>(0, 16),
                    ConfigTags.Advanced
                    ));

            //TimerNotificationInterval = Config.AddSetting<int>(
            //   timerSection,
            //   "Timer Interval",
            //   15,
            //   new ConfigDescription(
            //       "Time between chat notifications and reminders",
            //       new AcceptableValueList<int>(15, 30),
            //       ConfigTags.Advanced
            //       ));

            //TimeUntilVoteOverride = Config.AddSetting<int>(
            //   timerSection,
            //   "Timer Limit",
            //   60,
            //   new ConfigDescription(
            //       "Time after first vote until lock is overriden and you can use teleporter",
            //       new AcceptableValueList<int>(30, 60, 90, 120),
            //       ConfigTags.Advanced
            //       ));
            #endregion

            #region HookRegistration
            //Main hooks - triggers restriction logics
            On.RoR2.TeleporterInteraction.OnInteractionBegin += TeleporterInteraction_OnInteractionBegin;
            On.RoR2.TeleporterInteraction.OnStateChanged += TeleporterInteraction_OnStateChanged;
            On.RoR2.Interactor.PerformInteraction += Interactor_PerformInteraction;
            On.RoR2.PlayerCharacterMasterController.OnBodyDeath += PlayerCharacterMasterController_OnBodyDeath;
            VoteController.PlayerRegistered += VoteController_PlayerRegistered;

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

        private void VoteController_PlayerRegistered(object sender, EventArgs e)
        {
            var args = (PlayerRegisteredEventArgs)e;
            Message.SendColoured($"{args.NumberOfRegisteredPlayers}/{args.NumberOfVotesNeeded} players ready.", Colours.Green);
        }

        #region ControllerTidyUp  
        private void StopAll()
        {
            VoteController.Reset();
            TimerController.Stop();
        }

        private void PlayerCharacterMasterController_OnBodyDeath(On.RoR2.PlayerCharacterMasterController.orig_OnBodyDeath orig, PlayerCharacterMasterController self)
        {
            StopAll();
            Message.SendColoured("Player died. Reinstating restriction.", Colours.Red);
            orig(self);
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

        private void Run_BeginGameOver(On.RoR2.Run.orig_BeginGameOver orig, Run self, GameResultType gameResultType)
        {
            if (self.isGameOverServer)
            {
                StopAll();                
            }
            orig(self, gameResultType);
        }
        #endregion

        #region VoteRegisterMethods
        private void TeleporterInteraction_OnInteractionBegin(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, TeleporterInteraction self, Interactor activator)
        {
            VoteController.RegisterPlayer(GetNetworkUserFromInteractor(activator));
            if (VoteController.VotesReady)
            {
                StopAll();
                orig(self, activator);
            }
        }

        private void Interactor_PerformInteraction(On.RoR2.Interactor.orig_PerformInteraction orig, Interactor self, GameObject interactableObject)
        { 
            if (InteractableObjectNames.IsRestictedInteractableObject(interactableObject.name))
            {
                VoteController.RegisterPlayer(GetNetworkUserFromInteractor(self));
                if (VoteController.VotesReady)
                {
                    StopAll();
                    orig(self, interactableObject);
                }
            }
            else
            {
                orig(self, interactableObject);
            }
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
