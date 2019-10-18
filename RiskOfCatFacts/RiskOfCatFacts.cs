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
        private System.Random random = new System.Random();
        private Timer timer;
        private double interval = 60 * 1000;

        private ConfigEntry<bool> CatFactsEnabled;

        public void Awake()
        {
            CatFactsEnabled = Config.AddSetting<bool>(
                "CatFacts",
                "ReceiveCatFacts",
                true,
                new ConfigDescription("Enable/Disable receiving CatFacts"));

            //Chat.onChatChanged += Chat_onChatChanged;
            RoR2.Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
            RoR2.Run.onRunStartGlobal += Run_onRunStartGlobal;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            On.RoR2.Run.BeginStage += Run_BeginStage;
        }

        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            if (CatFactsEnabled.Value
                && self.stageClearCount > 0
                && timer == null)
            {
                Start();
            }
            orig(self);
        }

        private void Run_onRunStartGlobal(Run obj)
        {
            if (CatFactsEnabled.Value)
            {
                Start();
            }
        }

        private void Run_onRunDestroyGlobal(Run obj)
        {
            Stop();
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

        private void SendCatFact()
        {
            var index = random.Next(0, CatFacts.Facts.Count);
            Message.SendColoured(CatFacts.Facts[index], Colours.LightBlue, "CatFact");
        }

        private void FakeUnsubscribe()
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

        private void Start()
        {
            timer = new Timer
            {
                Interval = interval,
                AutoReset = true,
                Enabled = false
            };                
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Stop()
        {
            interval = 60 * 1000;
            if(timer != null)
            {
                timer.Dispose();
            }
        }

        private static Regex ParseChatLog => new Regex(@"<color=#[0-9a-f]{6}><noparse>(?<name>.*?)</noparse>:\s<noparse>(?<message>.*?)</noparse></color>");
        private void Chat_onChatChanged()
        {
            if (!CatFactsEnabled.Value)
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
                            Message.SendColoured("Command not recognised.", Colours.Red);
                            Message.SendColoured("You are already subscribed to CatFacts. Did you mean to increase frequency of facts? Increasing frequency. Type \"NO\" if you wish to undo this action.", Colours.Orange);
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
