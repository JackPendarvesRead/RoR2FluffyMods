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
using R2API;
using R2API.Utils;
using System.Collections.Generic;

namespace RiskOfVampirism
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.RiskOfVampirism", "RiskOfVampirism", "1.0.0")]
    public class RiskOfVampirism : BaseUnityPlugin
    {
        private List<NetworkUserId> Vampires = new List<NetworkUserId>();        
        private static ConfigWrapper<float> Leech { get; set; }

        public void Awake()
        {
            #region ConfigWrappers
            Leech = Config.Wrap<float>(
                "Leech",
                "LeechCount",
                "The amount of leech given to vampires",
                0.2f);
            #endregion

            On.RoR2.Console.Awake += (orig, self) =>
            {
                CommandHelper.RegisterCommands(self);
                orig(self);
            };

            On.RoR2.Run.Start += Run_Start;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
        {
            Vampires.Clear();
            orig(self);
            foreach (var nu in NetworkUser.readOnlyInstancesList)
            {
                if (!Vampires.Contains(nu.Network_id))
                {
                    Vampires.Add(nu.Network_id);
                }
            }
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            var attacker = damageReport.damageInfo.attacker.GetComponent<CharacterBody>();
            if(CheckIsPlayer(attacker))
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
                var player = NetworkUser.readOnlyInstancesList.Where(nu => nu.GetCurrentBody() == attacker).FirstOrDefault();

                if (player != null 
                    && Vampires.Contains(player.Network_id) 
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
                var player = NetworkUser.readOnlyInstancesList.Where(nu => nu.GetCurrentBody() == body).FirstOrDefault();
                if(CheckIsPlayer(body) 
                && Vampires.Contains(player.Network_id))
                {
                    //var levelBoost = 2 * (body.level - 1);
                    return -1f * body.maxHealth / 60 * (body.inventory.GetItemCount(ItemIndex.LunarDagger) + 1);
                }
                else
                {
                    return a;
                }                
            });
        }      
        
        private bool CheckIsPlayer(CharacterBody body)
        {
            var player = NetworkUser.readOnlyInstancesList.Where(nu => nu.GetCurrentBody() == body).FirstOrDefault();
            if(player != null)
            {
                return true;
            }
            else
            {
                return false;
            }

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

        ///// <summary>
        ///// Sets the decay time given at start of run.
        ///// </summary>
        ///// <param name="args">args\[0\]=Value(int)</param>
        //[ConCommand(commandName = "vampire_decay", flags = ConVarFlags.ExecuteOnServer, helpText = "Sets decay time given at start of run. args[0]=Value(int).")]
        //private static void VampireDecaySet(ConCommandArgs args)
        //{
        //    try
        //    {
        //        DecayTime.Value = Int32.Parse(args[0]);
        //        Debug.Log($"decay time set to {args[0]}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.LogError(ex);
        //    }
        //}
        #endregion
    }
}
