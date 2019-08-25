using BepInEx;
using MonoMod.Cil;
using RoR2;
using R2API.Utils;
using UnityEngine;
using System;

namespace RoR2FluffyMods
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.RoR2FluffyMods", "RoR2FluffyMods", "0.0.0")]
    public class TestingStuff : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.GlobalEventManager.OnPlayerCharacterDeath += GlobalEventManager_OnPlayerCharacterDeath;
        }

        private void GlobalEventManager_OnPlayerCharacterDeath(On.RoR2.GlobalEventManager.orig_OnPlayerCharacterDeath orig, 
            GlobalEventManager self, 
            DamageInfo damageInfo, 
            GameObject victim, 
            NetworkUser victimNetworkUser)
        {
            var attacker = damageInfo.attacker;
            var body = victimNetworkUser.GetCurrentBody();


            var mask = new ProcChainMask();
            body.healthComponent.Heal(500, mask);

            Debug.Log($"Attacker name: {attacker.name}");


            orig(self, damageInfo, victim, victimNetworkUser);
        }
    }
}
