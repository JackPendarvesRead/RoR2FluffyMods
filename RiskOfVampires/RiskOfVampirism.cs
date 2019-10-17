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
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.RiskOfVampirism", "RiskOfVampirism", "1.0.1")]
    public class RiskOfVampirism : BaseUnityPlugin
    {      
        private static ConfigWrapper<float> Leech { get; set; }
        private static ConfigWrapper<int> DecayTime { get; set; }
        private static ConfigWrapper<bool> Vampire { get; set; }

        public void Awake()
        {
            #region ConfigWrappers
            Leech = Config.Wrap<float>(
                "Stats",
                "Leech",
                "<float> The amount leech given to vampires (% damage)",
                0.15f);

            DecayTime = Config.Wrap<int>(
                "Stats",
                "DecayTime",
                "<int> The time(s) for player to degenerate health to zero",
                60);

            Vampire = Config.Wrap<bool>(
               "Vampire",
               "IsAVampire",
               "<bool> Set to true to be a vampire",
               true);
            #endregion
            
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            var attacker = damageReport.damageInfo.attacker.GetComponent<CharacterBody>();
            var player = GetPlayer(attacker);
            if (player != null && Vampire.Value)
            {
                attacker.baseMaxHealth += 1;
            }
            orig(self, damageReport);
        }        

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            if (!damageInfo.procChainMask.HasProc(ProcType.HealOnHit))
            {
                var attacker = damageInfo.attacker.GetComponent<CharacterBody>();
                var healthComponent = attacker.GetComponent<HealthComponent>();
                var player = GetPlayer(attacker);
                if (player != null 
                    && Vampire.Value
                    && (bool)((UnityEngine.Object)healthComponent))
                {                    
                    var procChainMask = damageInfo.procChainMask;
                    procChainMask.AddProc(ProcType.HealOnHit);

                    //var survivorCoefficient = GetSurvivorCoefficient(attacker); 
                    //var num = (double)healthComponent.Heal((5 * damageInfo.procCoefficient + attacker.level / 2) * survivorCoefficient, procChainMask, true); 
                    
                    var num = (double)healthComponent.Heal(damageInfo.damage * Leech.Value, procChainMask, true);                    
                }                             
            }
            orig(self, damageInfo, victim);
        }
        
        private void CharacterBody_RecalculateStats(ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdarg(0),
                x => x.MatchLdfld(out FieldReference fr1),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld(out FieldReference fr2),
                x => x.MatchLdloc(27),
                x => x.MatchMul(),
                x => x.MatchAdd(),
                x => x.MatchStloc(36)
                );
            c.Index -= 1;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<float, CharacterBody, float>>((a, body) =>
            {
                var player = GetPlayer(body);
                if(player != null && Vampire.Value)
                {
                    return -1f * body.maxHealth / DecayTime.Value * (body.inventory.GetItemCount(ItemIndex.LunarDagger) + 1);
                }
                else
                {
                    return a;
                }                
            });
        }    

        private float GetSurvivorCoefficient(CharacterBody body)
        {
            if (body.name.ToLower().StartsWith("commando")
                || body.name.ToLower().StartsWith("multi"))
            {
                return 0.5f;
            }
            if (body.name.ToLower().StartsWith("engineer")
                || body.name.ToLower().StartsWith("mage")
                || body.name.ToLower().StartsWith("treebot"))
            {
                return 3f;
            }
            return 1f;
        }

        private NetworkUser GetPlayer(CharacterBody body)
        {
            return NetworkUser.readOnlyInstancesList.Where(nu => nu.GetCurrentBody() == body).FirstOrDefault();
        }

        #region ConsoleCommands
        /// <summary>
        /// Sets the number of leech items given at start of run.
        /// </summary>
        /// <param name="args">args\[0\]=Value(int)</param>
        [ConCommand(commandName = "vampire_leech", flags = ConVarFlags.ExecuteOnServer, helpText = "Sets the number of leech items given at start of run. args[0]=Value(int).")]
        private static void VampireLeechSet(ConCommandArgs args)
        {
            try
            {
                Leech.Value = float.Parse(args[0]);
                Debug.Log($"Leech set to {args[0]}");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">args[0]=bool</param>
        [ConCommand(commandName = "vampire_role", flags = ConVarFlags.ExecuteOnServer, helpText = "Sets the number of leech items given at start of run. args[0]=Value(int).")]
        private static void VampireRole(ConCommandArgs args)
        {
            try
            {
                switch (args[0])
                {
                    case "1":
                    case "true":
                    case "t":
                        Vampire.Value = true;
                        Debug.Log("You are now a vampire");
                        break;

                    case "0":
                    case "false":
                    case "f":
                        Vampire.Value = false;
                        Debug.Log("You are no longer a vampire");
                        break;

                    default:
                        throw new Exception("Command arguements not valid.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        /// <summary>
        /// Sets the decay time given at start of run.
        /// </summary>
        /// <param name="args">args\[0\]=Value(int)</param>
        [ConCommand(commandName = "vampire_decay", flags = ConVarFlags.ExecuteOnServer, helpText = "Sets decay time given at start of run. args[0]=Value(int).")]
        private static void VampireDecaySet(ConCommandArgs args)
        {
            try
            {
                DecayTime.Value = Int32.Parse(args[0]);
                Debug.Log($"decay time set to {args[0]}");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        #endregion
    }
}
