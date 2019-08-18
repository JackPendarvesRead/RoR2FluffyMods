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
            //On.RoR2.DamageNumberManager.SpawnDamageNumber += DamageNumberManager_SpawnDamageNumber;
            IL.RoR2.DamageNumberManager.SpawnDamageNumber += DamageNumberManager_SpawnDamageNumber1;
        }

        private void DamageNumberManager_SpawnDamageNumber1(ILContext il)
        {
            var c = new ILCursor(il);
            c.EmitDelegate<Action>(() => { Debug.Log("ACTION ACTION!"); });
        }

        private void DamageNumberManager_SpawnDamageNumber(On.RoR2.DamageNumberManager.orig_SpawnDamageNumber orig, 
            DamageNumberManager self, 
            float amount,
            Vector3 position, 
            bool crit,
            TeamIndex teamIndex, 
            DamageColorIndex damageColorIndex)
        {
            orig(self,            
                amount,           
                position, 
                crit, 
                teamIndex, 
                damageColorIndex);
        }
    }
}
