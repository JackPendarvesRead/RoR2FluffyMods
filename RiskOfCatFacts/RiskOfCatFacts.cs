using BepInEx;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Timers;
using BepInEx.Configuration;

namespace RiskOfCatFacts
{
    [BepInPlugin("com.FluffyMods.RiskOfCatFacts", "RiskOfCatFacts", "2.0.0")]
    public class RiskOfCatFacts : BaseUnityPlugin
    {
        private ConfigEntry<bool> CatFactsEnabled;
        private ConfigEntry<bool> FactUnsubscribeCommands;
        private ConfigEntry<int> CatFactInterval;
        private System.Random random = new System.Random();
        private float currentTime;
        private bool timerRunning = false;
        private float unsubscribePenalty = 1;
        private float interval => (float)CatFactInterval.Value / unsubscribePenalty;

        public void Awake()
        {
            const string catFactSection = "CatFacts";

            FactUnsubscribeCommands = Config.AddSetting<bool>(
                catFactSection,
                "FactUnsubscribeCommands",
                true,
                new ConfigDescription("Disable this to stop the fake unsubscribe chat commands"));

            CatFactsEnabled = Config.AddSetting<bool>(
                catFactSection,
                "ReceiveCatFacts",
                true,
                new ConfigDescription("Enable/Disable receiving CatFacts"));

            CatFactInterval = Config.AddSetting<int>(
                catFactSection,
                "CatFactInterval",
                60,
                new ConfigDescription(
                    "The time(s) between receiving CatFacts",               
                    new AcceptableValueRange<int>(10,120))
                    );

            Chat.onChatChanged += Chat_onChatChanged;
            RoR2.Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
            RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            RoR2.SceneDirector.onPostPopulateSceneServer += SceneDirector_onPostPopulateSceneServer;
        }       

        
        public void Update()
        {
            if(timerRunning)
            {
                if (!CatFactsEnabled.Value)
                {
                    Stop();
                }
                currentTime -= Time.deltaTime;
                if(currentTime <= 0)
                {
                    currentTime = interval;
                }
            }
        }

        private void Start()
        {
            if (CatFactsEnabled.Value)
            {
                timerRunning = true;
                currentTime = interval;
            }            
        }

        private void Stop()
        {
            timerRunning = false;
            unsubscribePenalty = 1;
        }

        private void SceneDirector_onPostPopulateSceneServer(SceneDirector obj)
        {
            Start();            
        }

        private void Run_onRunDestroyGlobal(Run obj)
        {
            Stop();
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport obj)
        {
            if (CatFactsEnabled.Value
                && obj.victim.body.isChampion)
            {
                Message.Send("Meow meow Me-WOW! You killed a champion! =^o_o^=");
                SendCatFact();
            }
        }           

        private void SendCatFact()
        {
            var index = random.Next(0, CatFacts.Facts.Count);
            Message.SendColoured(CatFacts.Facts[index], Colours.LightBlue, "CatFact");
        }

        private void FakeUnsubscribe()
        {
            Message.SendColoured("Command not recognised.", Colours.Red);
            Message.SendColoured("Thank you for subscribing to CatFacts! We will double your fact rate free of charge!", Colours.Green);
            unsubscribePenalty++;
        }

        private void FakeUnsubscribe2()
        {
            Message.SendColoured("Command not recognised.", Colours.Red);
            Message.SendColoured("You are already subscribed to CatFacts. Did you mean to increase frequency of facts? Increasing frequency. Type \"NO\" if you wish to undo this action.", Colours.Orange);
        }

        private static Regex ParseChatLog => new Regex(@"<color=#[0-9a-f]{6}><noparse>(?<name>.*?)</noparse>:\s<noparse>(?<message>.*?)</noparse></color>");
        private void Chat_onChatChanged()
        {
            if (!CatFactsEnabled.Value
                || !FactUnsubscribeCommands.Value)
            {
                return;
            }
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
                        case "cat":
                        case "fact":
                            SendCatFact();
                            break;
                            
                        case "dog":
                            FakeUnsubscribe();
                            break;

                        case "unsubscribe":
                            FakeUnsubscribe2();
                            break;

                        case "yes":
                        case "no":
                        case "stop":
                        case "undo":
                        case "please":
                            FakeUnsubscribe();
                            break;

                        case "seriously please actually stop this is enough":
                            Stop();
                            break;                            
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }       
    }
}
