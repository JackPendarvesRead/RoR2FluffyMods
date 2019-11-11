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
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class RiskOfCatFacts : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "RiskOfCatFacts";
        private const string pluginVersion = "3.0.0";

        private ConfigEntry<bool> CatFactsEnabled;
        private ConfigEntry<bool> FactUnsubscribeCommands;
        private ConfigEntry<int> CatFactInterval;
        private System.Random random = new System.Random();
        private float currentTime;
        private bool timerRunning = false;
        private float unsubscribePenalty = 0;

        private float interval
        {
            get
            {
                var x = (float)CatFactInterval.Value / Mathf.Pow(2, unsubscribePenalty);
                return x > 1 ? x : 1;
            }
        }

        public void Awake()
        {
            const string catFactSection = "CatFacts";

            FactUnsubscribeCommands = Config.Bind<bool>(
                catFactSection,
                "FactUnsubscribeCommands",
                true,
                new ConfigDescription("Disable this to stop the fake unsubscribe chat commands"));

            CatFactsEnabled = Config.Bind<bool>(
                "Enable/Disable Mod",
                "ReceiveCatFacts",
                true,
                new ConfigDescription("Enable/Disable receiving CatFacts"));

            CatFactInterval = Config.Bind<int>(
                catFactSection,
                "CatFactInterval",
                60,
                new ConfigDescription(
                    "The time(s) between receiving CatFacts",               
                    new AcceptableValueRange<int>(10,120))
                    );

            Chat.onChatChanged += Chat_onChatChanged;
            RoR2.Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
            RoR2.GlobalEventManager.onCharacterDeathGlobal += SendCatFactOnChampionKill;
            RoR2.SceneDirector.onPostPopulateSceneServer += SceneDirector_onPostPopulateSceneServer;
        }        

        public void Update()
        {
            if(timerRunning)
            {
                if (!CatFactsEnabled.Value)
                {
                    StopCatFacts();
                }
                currentTime -= Time.deltaTime;
                if(currentTime <= 0)
                {
                    SendCatFact();
                }
            }
        }

        private void StartCatFacts()
        {
            if (CatFactsEnabled.Value)
            {
                timerRunning = true;
                currentTime = interval;
            }            
        }

        private void StopCatFacts()
        {
            timerRunning = false;
            unsubscribePenalty = 0;
        }

        private void SceneDirector_onPostPopulateSceneServer(SceneDirector obj)
        {
            if (RoR2.Run.instance)
            {
                StartCatFacts();
            }
        }

        private void Run_onRunDestroyGlobal(Run obj)
        {
            StopCatFacts();
        }

        private void SendCatFactOnChampionKill(DamageReport obj)
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
            currentTime = interval;
            var index = random.Next(0, CatFacts.Facts.Count);
            Message.SendColoured(CatFacts.Facts[index], Colours.LightBlue, "CatFact");
        }

        private void FakeUnsubscribe()
        {
            Message.SendColoured("Command not recognised.", Colours.Red);
            Message.SendColoured("Thank you for subscribing to CatFacts! We will double your fact rate free of charge!", Colours.Green);
            unsubscribePenalty++;
            currentTime = interval;
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
                || !FactUnsubscribeCommands.Value
                || !Chat.readOnlyLog.Any())
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
                            StopCatFacts();
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
