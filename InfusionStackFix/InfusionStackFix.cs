using BepInEx;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace InfusionStackFix
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.InfusionStackFix", "InfusionStackFix", "1.0.0")]
    public class InfusionStackFix : BaseUnityPlugin
    {
        public void Awake()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
        }

        private void GlobalEventManager_OnCharacterDeath(MonoMod.Cil.ILContext il)
        {
            var c = new ILCursor(il);

        }
    }
}
