using BepInEx;
using BepInEx.Configuration;
using RoR2;

namespace WikiOfRain
{
    [BepInPlugin("com.FluffyMods.WikiOfRain", "WikiOfRain", "1.0.0")]
    public class MotherfuckingFungus : BaseUnityPlugin
    {
        private static ConfigEntry<int> Interval;
        private static ConfigEntry<bool> ShouldShowWiki; 
        const string randomWikiUrl = @"https://en.wikipedia.org/wiki/Special:Random";
        WikipediaMessageController controller;

        public void Awake()
        {
            Interval = Config.AddSetting<int>(
                "Timer",
                "Interval(s)",
                10,
                new ConfigDescription("Time(s) between each wikipedia entry being added to chat"));

            ShouldShowWiki = Config.AddSetting<bool>(
                "Wiki",
                "ReadWiki",
                true,
                new ConfigDescription("Enable/Disable reading wikipedia"));

            On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.Run.AdvanceStage += Run_AdvanceStage;
            RoR2.Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
            RoR2.Run.OnServerGameOver += Run_OnServerGameOver;
        }

        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            orig(self);
            controller = new WikipediaMessageController(randomWikiUrl, Interval.Value);
            controller.Start();
        }

        private void Run_AdvanceStage(On.RoR2.Run.orig_AdvanceStage orig, Run self, SceneDef nextScene)
        {
            orig(self, nextScene);            
            controller.Stop();
        }

        private void Run_OnServerGameOver(Run arg1, GameResultType arg2)
        {
            controller.Stop();
        }

        private void Run_onRunDestroyGlobal(Run obj)
        {
            controller.Stop();
        }
    }
}
