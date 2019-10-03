using BepInEx;
using MonoMod.Cil;
using RoR2;
using R2API.Utils;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Timers;
using BepInEx.Configuration;

namespace RiskOfCatFacts
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.RiskOfCatFacts", "RiskOfCatFacts", "1.1.0")]
    public class RiskOfCatFacts : BaseUnityPlugin
    {
        private System.Random random;
        private Timer timer;
        private double interval = 60 * 1000;

        private static ConfigEntry<bool> CatFactsEnabled { get; set; }

        public void Awake()
        {
            CatFactsEnabled = Config.AddSetting<bool>(
                "CatFacts",
                "ReceiveCatFacts",
                true,
                new ConfigDescription("Enable/Disable receiving CatFacts"));


            random = new System.Random();

            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;

            Chat.onChatChanged += Chat_onChatChanged;
            On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.Run.BeginGameOver += Run_BeginGameOver;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            if (CatFactsEnabled.Value
                && damageReport.victim.body.isChampion)
            {
                Message.Send("Meow meow Me-WOW! You killed a champion! =^o_o^=");
                SendCatFact();
            }            
            orig(self, damageReport);
        }

        private void Run_BeginGameOver(On.RoR2.Run.orig_BeginGameOver orig, Run self, GameResultType gameResultType)
        {
            Stop();
            orig(self, gameResultType);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (CatFactsEnabled.Value)
            {
                SendCatFact();
            }
            else
            {
                Stop();
            }
        }

        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            if (CatFactsEnabled.Value
                && !timer.Enabled)
            {
                timer.Interval = interval;
                timer.Start();
            }
            orig(self);
        }        

        private void SendCatFact()
        {
            var index = random.Next(0, CatFacts.Facts.Count);
            Message.SendColoured(CatFacts.Facts[index], Colours.LightBlue, "CatFact");
        }

        private void Unsubscribe()
        {
            Message.SendColoured("Command not recognised.", Colours.Red);
            Message.SendColoured("Thank you for subscribing to CatFacts! We will double your fact rate free of charge!", Colours.Green);
            HalfTimerTime();
        }

        private void HalfTimerTime()
        {
            if(interval > 1)
            {
                interval /= 2;
                timer.Interval = interval;
            }            
        }

        private void Stop()
        {
            interval = 60 * 1000;
            timer.Stop();
        }

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
                        case "cat":
                        case "fact":
                            SendCatFact();
                            break;
                            
                        case "dog":
                            Unsubscribe();
                            break;

                        case "unsubscribe":
                            Message.SendColoured("Command not recognised.", Colours.Red);
                            Message.SendColoured("You are already subscribed to CatFacts. Did you mean to increase frequency of facts? Increasing frequency. Type \"NO\" if you wish to undo this action.", Colours.Orange);
                            break;

                        case "yes":
                        case "no":
                        case "stop":
                        case "undo":
                        case "please":
                            Unsubscribe();
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
