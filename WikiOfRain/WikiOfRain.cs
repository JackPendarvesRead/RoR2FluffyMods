using BepInEx;
using RoR2;

namespace WikiOfRain
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.WikiOfRain", "WikiOfRain", "1.0.0")]
    public class MotherfuckingFungus : BaseUnityPlugin
    {
        //private static ConfigEntry<int> Interval;

        const string randomWikiUrl = @"https://en.wikipedia.org/wiki/Special:Random";

        WikipediaMessageController controller = new WikipediaMessageController(randomWikiUrl, 10);
        public void Awake()
        {
            //Interval = Config.AddSetting<int>(
            //    "Time",
            //    "Time",
            //    10,
            //    new ConfigDescription("Time"));

            On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.Run.AdvanceStage += Run_AdvanceStage;
            RoR2.Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
            RoR2.Run.OnServerGameOver += Run_OnServerGameOver;
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

        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            orig(self);
            //controller = new WikipediaMessageController(randomWikiUrl, Interval.Value);
            controller.Start();
        }
    }
}
