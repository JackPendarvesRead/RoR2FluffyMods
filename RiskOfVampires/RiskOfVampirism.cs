using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace RiskOfVampirism
{
    [BepInPlugin("com.FluffyMods.RiskOfVampirism", "RiskOfVampirism", "2.0.0")]
    public class RiskOfVampirism : BaseUnityPlugin
    {      
        private static ConfigEntry<float> Leech { get; set; }
        private static ConfigEntry<int> DecayTime { get; set; }
        private static ConfigEntry<int> DegenerationThreshold { get; set; }
        private static ConfigEntry<bool> IsVampire { get; set; }

        public void Awake()
        {
            #region ConfigSetup
            const string statsSection = "Stats";

            Leech = Config.AddSetting<float>(
                statsSection,
                "%LifeLeech",
                0.15f,
                new ConfigDescription(
                    "The amount leech given to vampires (% damage)",
                    new AcceptableValueRange<float>(0, 1)));

            DecayTime = Config.AddSetting<int>(
                statsSection,
                "HealthDecayTime",
                60,
                "The time(s) for player to degenerate health to zero");

            DegenerationThreshold = Config.AddSetting<int>(
                statsSection,
                "DegenerationThreshold",
                5,
                "You will not degenerate below this threshold number");

            IsVampire = Config.AddSetting<bool>(
               "Vampire",
               "IsAVampire",
               true,
               "Set to true to be a vampire");
            #endregion

            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        // GAIN MAX HEALTH ON KILL METHOD
        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            var attacker = damageReport.damageInfo.attacker.GetComponent<CharacterBody>();
            var player = GetPlayer(attacker);
            if (player != null && IsVampire.Value)
            {
                attacker.baseMaxHealth += 1;
            }
            orig(self, damageReport);
        }        

        // LIFESTEAL METHOD
        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            if (!damageInfo.procChainMask.HasProc(ProcType.HealOnHit))
            {
                var attacker = damageInfo.attacker.GetComponent<CharacterBody>();
                var healthComponent = attacker.GetComponent<HealthComponent>();
                var player = GetPlayer(attacker);
                if (player != null 
                    && IsVampire.Value
                    && (bool)((UnityEngine.Object)healthComponent))
                {                    
                    var procChainMask = damageInfo.procChainMask;
                    procChainMask.AddProc(ProcType.HealOnHit);

                    // var survivorCoefficient = GetSurvivorCoefficient(attacker); 
                    // var num = (double)healthComponent.Heal((5 * damageInfo.procCoefficient + attacker.level / 2) * survivorCoefficient, procChainMask, true); 
                    
                    var num = (double)healthComponent.Heal(damageInfo.damage * Leech.Value, procChainMask, true);                    
                }                             
            }
            orig(self, damageInfo, victim);
        }
        
        // SET REGEN (HEALTH DECAY) METHOD
        private void CharacterBody_RecalculateStats(ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(x => x.MatchCallvirt<CharacterBody>("set_regen"));
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<float, CharacterBody, float>>((a, body) =>
            {
                var player = GetPlayer(body);
                if(player != null && IsVampire.Value)
                {
                    if (body.healthComponent.health <= DegenerationThreshold.Value)
                    {
                        return 0;
                    }
                    return -1f * body.maxHealth / DecayTime.Value * (body.inventory.GetItemCount(ItemIndex.LunarDagger) + 1);
                }
                else
                {
                    return a;
                }                
            });
        }

        // Threshold logic for degeneration
        bool degenerating = true;
        private void CharacterBody_FixedUpdate(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            orig(self);
            var player = GetPlayer(self);
            if (player != null)
            {
                var health = self.healthComponent.health;
                if (degenerating
                    && self.healthComponent.health <= DegenerationThreshold.Value)
                {
                    degenerating = false;
                    self.RecalculateStats();
                }
                if (!degenerating
                    && self.healthComponent.health > DegenerationThreshold.Value)
                {
                    degenerating = true;
                    self.RecalculateStats();
                }
            }
        }

        private NetworkUser GetPlayer(CharacterBody body)
        {
            return NetworkUser.readOnlyInstancesList
                .Where(nu => nu.GetCurrentBody() == body)
                .FirstOrDefault();
        }

        //private float GetSurvivorCoefficient(CharacterBody body)
        //{
        //    Debug.Log($"VAMPIRE BODY = {body.name}");
        //    if (body.name.ToLower().StartsWith(SurvivorIndex.Commando.ToString().ToLower())
        //        || body.name.ToLower().StartsWith(SurvivorIndex.Toolbot.ToString().ToLower()))
        //    {
        //        Debug.Log("RETURN 0.5f");
        //        return 0.5f;
        //    }
        //    if (body.name.ToLower().StartsWith(SurvivorIndex.Engi.ToString().ToLower())
        //        || body.name.ToLower().StartsWith(SurvivorIndex.Mage.ToString().ToLower())
        //        || body.name.ToLower().StartsWith(SurvivorIndex.Treebot.ToString().ToLower()))
        //    {
        //        Debug.Log("RETURN 3f");
        //        return 3f;
        //    }
        //    Debug.Log("RETURN 1f");
        //    return 1f;
        //}
    }
}
