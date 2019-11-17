using BepInEx;
using RoR2;
using System;

namespace FixBleedError
{
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class FixBleedError : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "Test";
        private const string pluginVersion = "1.0.0";

        public void Awake()
        {
            //RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            On.RoR2.DotController.FixedUpdate += DotController_FixedUpdate;
        }

        private void DotController_FixedUpdate(On.RoR2.DotController.orig_FixedUpdate orig, DotController self)
        {            
            if (self.victimObject)
            {
                try
                {                
                    var body = self.victimObject.GetComponent<CharacterBody>();
                    if (!body.healthComponent.alive)
                    {
                        Logger.LogInfo($"Name={body.name}: NetId={body.netId} Victim body not alive in fixed update");
                        self.enabled = false;
                    }
                }
                catch
                {
                    Logger.LogInfo("failed checking if victim is alive in fixed update");
                }
            }
            else
            {
                Logger.LogInfo("Victim is null");
            }
            orig(self);
        }

        private void GlobalEventManager_onCharacterDeathGlobal(RoR2.DamageReport obj)
        {
            try
            {
                DotController.RemoveAllDots(obj.victim.gameObject);
                Logger.LogInfo($"Removed all dots from {obj.victim.name}");
                //var component = obj.victim.gameObject.GetComponent<DotController>();
                //if (component)
                //{
                //    component = null;
                //}
            }
            catch (Exception ex)
            {
                Logger.LogInfo("Failed to fix bleed error!");
                Logger.LogError(ex);
            }
        }
    }
}
