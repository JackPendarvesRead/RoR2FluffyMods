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
using DeployableOwnerInformation.Component;
using DeployableOwnerInformation.Extension;

namespace RiskOfVampirism
{
    [BepInDependency("com.FluffyMods.FluffyLabsConfigManagerTools")]
    [PluginMetadata("com.FluffyMods.RiskOfVampirism", "RiskOfVampirism", "2.0.0")]
    public class RiskOfVampirism : BaseUnityPlugin
    {
        private ConfigEntry<float> Leech;
        private ConfigEntry<int> DecayTime;
        private ConfigEntry<int> DegenerationThreshold;
        private ConfigEntry<bool> GainsMaximumHealth;
        private ConfigEntry<bool> IsVampire;
        private ConfigEntry<bool> TurretsTransferLifeToOwner;
        private List<ConfigEntry<float>> SurvivorCoefficients;

        public void Start()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }

            #region ConfigSetup
            const string statsSection = "Stats";
            const string vampireSection = "RiskOfVampirsm";

            Leech = Config.Bind<float>(
                statsSection,
                "%LifeLeech",
                0.08f,
                new ConfigDescription(
                    "The amount leech given to vampires (% damage)",
                    new AcceptableValueRange<float>(0, 1)));

            DecayTime = Config.Bind<int>(
                statsSection,
                "HealthDecayTime",
                45,
                "The time(s) for player to degenerate health to zero");

            DegenerationThreshold = Config.Bind<int>(
                statsSection,
                "DegenerationThreshold",
                1,
                "You will not degenerate below this threshold number");

            GainsMaximumHealth = Config.Bind<bool>(
               vampireSection,
               "GainMaximumHealth",
               true,
               "Enable to gain +1 base max health for each kill you make");

            IsVampire = Config.Bind<bool>(
               vampireSection,
               "IsAVampire",
               true,
               "Set to true to be a vampire (Enable/Disable the mod)");

            TurretsTransferLifeToOwner = Config.Bind<bool>(
             vampireSection,
             "TurretsTransferLifeToOwner",
             true,
             "Set to true and turrets will lifesteal for the engineer (i.e. turret damage restores life to main engi body)");

            SurvivorCoefficients = GetSurvivorConfigEntries().ToList();
            #endregion

            RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            RoR2.GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        // LIFESTEAL METHOD
        private void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            var damageInfo = damageReport.damageInfo;
            if (IsVampire.Value
                && damageInfo != null
                && !damageInfo.procChainMask.HasProc(ProcType.HealOnHit))
            {
                var attacker = damageInfo.attacker?.GetComponent<CharacterBody>();
                if(attacker != null)
                {
                    var healthComponent = attacker.GetComponent<HealthComponent>();
                    //PlayerLifesteal
                    if (GetPlayer(attacker) != null
                        && (bool)((UnityEngine.Object)healthComponent))
                    {
                        var procChainMask = damageInfo.procChainMask;
                        procChainMask.AddProc(ProcType.HealOnHit);
                        var survivorCoefficient = GetSurvivorCoefficient(attacker);
                        var num = (double)healthComponent.Heal(damageInfo.damage * Leech.Value * survivorCoefficient, procChainMask, true);
                    }
                    //Turrets LifeSteal
                    if (TurretsTransferLifeToOwner.Value
                        && attacker.teamComponent.teamIndex == TeamIndex.Player
                        && attacker.name.ToLower().Contains("turret"))
                    {
                        var owner = attacker.GetOwnerInformation().OwnerBody;
                        var procChainMask = damageInfo.procChainMask;
                        procChainMask.AddProc(ProcType.HealOnHit);
                        var num = (double)owner.healthComponent.Heal(damageInfo.damage * Leech.Value, procChainMask, true);
                    }
                }                
            }
        }

        // GAIN MAX HEALTH ON KILL METHOD
        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (IsVampire.Value && GainsMaximumHealth.Value)
            {
                var attacker = damageReport.damageInfo.attacker?.GetComponent<CharacterBody>();
                if (attacker != null)
                {
                    var player = GetPlayer(attacker);
                    if (player != null)
                    {
                        attacker.baseMaxHealth += 1;
                    }
                }
            }
        }

        // SET REGEN (HEALTH DECAY) METHOD
        private void CharacterBody_RecalculateStats(ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(x => x.MatchCallvirt<CharacterBody>("set_regen"));
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<float, CharacterBody, float>>((a, body) =>
            {
                if(IsVampire.Value
                    && GetPlayer(body) != null)
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
            if (IsVampire.Value && GetPlayer(self) != null)
            {                
                var health = self.healthComponent.health;
                if (degenerating && health <= DegenerationThreshold.Value)
                {
                    degenerating = false;
                    self.RecalculateStats();
                }
                if (!degenerating && health > DegenerationThreshold.Value)
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

        private float GetSurvivorCoefficient(CharacterBody body)
        {
            return (from s in SurvivorCoefficients
                    where body.name.StartsWith(s.Definition.Key)
                    select s.Value).FirstOrDefault();
        }


        private IEnumerable<ConfigEntry<float>> GetSurvivorConfigEntries()
        {
            var survivors = RoR2.SurvivorCatalog.allSurvivorDefs;
            foreach (var survivor in survivors)
            {
                if (string.IsNullOrWhiteSpace(survivor.name))
                {
                    continue;
                }

                yield return Config.Bind<float>(
                    "SurvivorSpecificConfig",
                    survivor.name,
                    1.0f,
                    new ConfigDescription(
                        $"Lifesteal coefficient specific for {survivor.ToString()}. i.e. multiply lifesteal by this number if you are playing this survivor",
                        new AcceptableValueRange<float>(0, 2),
                        "Advanced"
                        ));
            }
        }
    }
}
