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
using FluffyLabsConfigManagerTools.Infrastructure;
using System.Net.NetworkInformation;

namespace RiskOfVampirism
{
    [BepInDependency(FluffyLabsConfigManagerTools.FluffyConfigLabsPlugin.PluginGuid)]
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class RiskOfVampirism : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "RiskOfVampirism";
        private const string pluginVersion = "3.0.0";

        private ConfigEntry<float> Leech;
        private ConfigEntry<int> DecayTime;
        private ConfigEntry<int> DegenerationThreshold;
        private ConditionalConfigEntry<int> MaxHealthGainOnKill;
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
                0.2f,
                new ConfigDescription(
                    "The amount leech given to vampires (% damage)",
                    new AcceptableValueRange<float>(0, 1)));

            DecayTime = Config.Bind<int>(
                statsSection,
                "HealthDecayTime",
                30,
                "The time(s) for player to degenerate health to zero");

            DegenerationThreshold = Config.Bind<int>(
                statsSection,
                "DegenerationThreshold",
                1,
                new ConfigDescription("You will not degenerate below this threshold number", null, "Advanced")
                );

            var conditionalUtil = new FluffyLabsConfigManagerTools.Util.ConditionalUtil(Config);
            MaxHealthGainOnKill = conditionalUtil.AddConditionalConfig<int>(
                vampireSection,
                "GainMaximumHealthOnKill",
                1,
                true,
                new ConfigDescription("Enable to gain +1 base max health for each kill you make"));

            IsVampire = Config.Bind<bool>(
               "Enable/Disable Mod",
               "IsAVampire",
               true,
               "Set to true to be a vampire (Enable/Disable the mod)");

            TurretsTransferLifeToOwner = Config.Bind<bool>(
             vampireSection,
             "TurretsTransferLifeToOwner",
             true,
             new ConfigDescription("Set to true and turrets will lifesteal for the engineer (i.e. turret damage restores life to main engi body)", null, "Advanced"));

            SurvivorCoefficients = GetSurvivorConfigEntries().ToList();
            InitialiseButtonConfigs();
            #endregion

            RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            RoR2.GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            RoR2.Run.onRunStartGlobal += Run_onRunStartGlobal;
            On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;

        }

        private void Run_onRunStartGlobal(Run obj)
        {
            Logger.LogInfo("THIS IS THE RUN START LOG");
            vampireHealthBonus = 0;
        }

        #region Buttons
        private void InitialiseButtonConfigs()
        {
            var buttonUtil = new FluffyLabsConfigManagerTools.Util.ButtonUtil(Config);
            buttonUtil.AddButtonConfig("Presets", "Presets", "Press these buttons to activate preset configurations",
                new Dictionary<string, Action>
                {
                    { "Default", DefaultImpl },
                    { "Dracula", DraculaImpl },
                    { "Relaxed", RelaxedImpl },
                    { "BloodGolem", BloodGolemImpl }
                });
        }

        private void DefaultImpl()
        {
            Leech.Value = 0.2f;
            DecayTime.Value = 30;
            DegenerationThreshold.Value = 1;
            MaxHealthGainOnKill.Condition = true;
            MaxHealthGainOnKill.Value = 1;
            IsVampire.Value = true;
            TurretsTransferLifeToOwner.Value = true;

        }

        private void DraculaImpl()
        {
            Leech.Value = 0.95f;
            DecayTime.Value = 5;
            DegenerationThreshold.Value = 1; 
            MaxHealthGainOnKill.Condition = true;
            MaxHealthGainOnKill.Value = 5;
            IsVampire.Value = true;
            TurretsTransferLifeToOwner.Value = true;
        }

        private void RelaxedImpl()
        {
            Leech.Value = 0.07f;
            DecayTime.Value = 60;
            DegenerationThreshold.Value = 1;
            MaxHealthGainOnKill.Condition = true;
            MaxHealthGainOnKill.Value = 1;
            IsVampire.Value = true;
            TurretsTransferLifeToOwner.Value = true;
        }

        private void BloodGolemImpl()
        {
            Leech.Value = 0.01f;
            DecayTime.Value = 300;
            DegenerationThreshold.Value = 1;
            MaxHealthGainOnKill.Condition = true;
            MaxHealthGainOnKill.Value = 50;
            IsVampire.Value = true;
            TurretsTransferLifeToOwner.Value = true;
        }
        #endregion

        // LIFESTEAL METHOD
        private void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            var damageInfo = damageReport.damageInfo;
            if (IsVampire.Value && 
                damageInfo != null && 
                !damageInfo.procChainMask.HasProc(ProcType.HealOnHit))
            {
                var attacker = damageInfo.attacker?.GetComponent<CharacterBody>();
                if(attacker != null)
                {
                    var healthComponent = attacker.GetComponent<HealthComponent>();
                    //PlayerLifesteal
                    if (GetPlayer(attacker) != null && (bool)((UnityEngine.Object)healthComponent))
                    {
                        var procChainMask = damageInfo.procChainMask;
                        procChainMask.AddProc(ProcType.HealOnHit);
                        var survivorCoefficient = GetSurvivorCoefficient(attacker);
                        _ = (double)healthComponent.Heal(damageInfo.damage * Leech.Value * survivorCoefficient, procChainMask, true);
                    }
                    //Turrets LifeSteal
                    if (TurretsTransferLifeToOwner.Value && 
                        attacker.teamComponent.teamIndex == TeamIndex.Player && 
                        attacker.name.ToLower().Contains("turret"))
                    {
                        try
                        {
                            var ownerBody = attacker.master.minionOwnership.ownerMaster.GetBody();
                            var procChainMask = damageInfo.procChainMask;
                            procChainMask.AddProc(ProcType.HealOnHit);
                            _ = (double)ownerBody.healthComponent.Heal(damageInfo.damage * Leech.Value, procChainMask, true);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("Error in turret lifesteal method in RiskOfVampirism mod. Please report to @fluffatron in #techsupport of RoR2Modding Discord and include this console output.");
                            Logger.LogError(ex);
                        }
                    }
                }                
            }
        }

        // GAIN MAX HEALTH ON KILL METHOD
        private int vampireHealthBonus = 0;
        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (IsVampire.Value && MaxHealthGainOnKill.Condition)
            {
                var attacker = damageReport.damageInfo.attacker?.GetComponent<CharacterBody>();
                if (attacker != null)
                {
                    var player = GetPlayer(attacker);
                    if (player != null)
                    {
                        vampireHealthBonus += MaxHealthGainOnKill.Value;
                        attacker.RecalculateStats();
                    }
                }
            }
        }

        // SET REGEN (HEALTH DECAY) METHOD and SET MAX HEALTH BONUS METHOD
        private void CharacterBody_RecalculateStats(ILContext il)
        {
            var c = new ILCursor(il);
            //MaxHealth
            c.GotoNext(x => x.MatchCallvirt<CharacterBody>("set_maxHealth"));
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<float, CharacterBody, float>>((currentMaxHealth, body) =>
            {
                if (IsVampire.Value && GetPlayer(body) != null)
                {
                    return currentMaxHealth + vampireHealthBonus;
                }
                else
                {
                    return currentMaxHealth;
                }

                
            });

            //Regen
            c.GotoNext(x => x.MatchCallvirt<CharacterBody>("set_regen"));
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<float, CharacterBody, float>>((currentRegen, body) =>
            {
                if(IsVampire.Value && GetPlayer(body) != null)
                {
                    if (body.healthComponent.health <= DegenerationThreshold.Value)
                    {
                        return 0;
                    }
                    return -1f * body.maxHealth / DecayTime.Value * (body.inventory.GetItemCount(ItemIndex.LunarDagger) + 1);
                }
                else
                {
                    return currentRegen;
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
            var survivors = RoR2.SurvivorCatalog.allSurvivorDefs.Where(x => !string.IsNullOrWhiteSpace(x.name));
            foreach (var survivor in survivors)
            {
                yield return Config.Bind<float>(
                    "SurvivorSpecificConfig",
                    survivor.name,
                    1.0f,
                    new ConfigDescription(
                        $"Lifesteal coefficient specific for {survivor}. i.e. multiply lifesteal by this number if you are playing this survivor",
                        new AcceptableValueRange<float>(0, 2),
                        "Advanced"
                        ));
            }
        }

        //private float GetDefaultSurvivorCoefficient(SurvivorIndex index)
        //{
        //    switch (index)
        //    {
        //        default:
        //            return 1.0f;
        //        case SurvivorIndex.Commando:
        //            break;
        //        case SurvivorIndex.None:
        //            break;
        //        case SurvivorIndex.Engi:
        //            break;
        //        case SurvivorIndex.Huntress:
        //            break;
        //        case SurvivorIndex.Bandit:
        //            break;
        //        case SurvivorIndex.Mage:
        //            break;
        //        case SurvivorIndex.Merc:
        //            break;
        //        case SurvivorIndex.Toolbot:
        //            break;
        //        case SurvivorIndex.Treebot:
        //            break;
        //        case SurvivorIndex.Loader:
        //            break;
        //        case SurvivorIndex.Croco:
        //            break;
        //        case SurvivorIndex.Count:
        //            break;
        //    }
        //    return 1.0f;
        //}
    }
}
