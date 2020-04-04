using BepInEx;
using System;
using UnityEngine;
using R2API.Utils;
using RoR2;

namespace Portal
{
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class Portal : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "Portal";
        private const string pluginVersion = "1.0.0";

        public void Awake()
        {
            On.RoR2.CharacterBody.Update += CharacterBody_Update;
        }

        private void CharacterBody_Update(On.RoR2.CharacterBody.orig_Update orig, RoR2.CharacterBody self)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Logger.LogInfo("Key is pressed");
                var startPosition = self.corePosition;
                var displacement = new Vector3(0, 50, 0);
            }
            orig(self);
        }
    }
}
