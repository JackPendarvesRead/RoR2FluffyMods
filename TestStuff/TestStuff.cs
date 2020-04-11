using BepInEx;
using RoR2;
using System;

namespace TestStuff
{
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class TestStuff : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "TestStuff";
        private const string pluginVersion = "1.0.0";

        public void Awake()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }

            On.RoR2.UI.PauseScreenController.OnDisable += PauseScreenController_OnDisable;
            On.RoR2.UI.PauseScreenController.OnEnable += PauseScreenController_OnEnable;
        }

        private void PauseScreenController_OnEnable(On.RoR2.UI.PauseScreenController.orig_OnEnable orig, RoR2.UI.PauseScreenController self)
        {
            Logger.LogInfo("ENABLE PAUSE");
            orig(self);
        }

        private void PauseScreenController_OnDisable(On.RoR2.UI.PauseScreenController.orig_OnDisable orig, RoR2.UI.PauseScreenController self)
        {
            Logger.LogInfo("DISABLE PAUSE");
            orig(self);
        }

    }
}
