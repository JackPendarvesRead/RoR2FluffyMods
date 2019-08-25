using BepInEx;
using MonoMod.Cil;
using RoR2;
using R2API.Utils;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Timers;

namespace RiskOfCatFacts
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.RiskOfCatFacts", "RiskOfCatFacts", "1.0.0")]
    public class RiskOfCatFacts : BaseUnityPlugin
    {
        private System.Random random;
        private Timer timer;

        public void Awake()
        {
            random = new System.Random();

            timer = new Timer(60 * 1000);
            timer.Elapsed += Timer_Elapsed;

            Chat.onChatChanged += Chat_onChatChanged;

            On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.Run.BeginGameOver += Run_BeginGameOver;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            if (damageReport.victim.body.isChampion)
            {
                Message.Send("Meow meow Me-WOW! You killed a champion! =^o_o^=");
                SendCatFact();
            }            
            orig(self, damageReport);
        }

        private void Run_BeginGameOver(On.RoR2.Run.orig_BeginGameOver orig, Run self, GameResultType gameResultType)
        {
            timer.Stop();
            orig(self, gameResultType);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SendCatFact();
        }

        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            if (!timer.Enabled)
            {
                timer.Start();
            }
            orig(self);
        }        

        private void SendCatFact()
        {
            var index = random.Next(0, CatFacts.Facts.Count);
            Message.SendColoured(CatFacts.Facts[index], Colours.LightBlue, "CatFact");
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
