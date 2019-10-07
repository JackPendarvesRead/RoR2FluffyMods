﻿using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using R2API.Utils;
using BepInEx.Configuration;

namespace ChronobaubleFix
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.ChronobaubleFix", "ChronobaubleFix", "1.0.0")]
    public class ChronobaubleFix : BaseUnityPlugin
    {
        private static ConfigEntry<float> SlowScalingCoefficient;
        private static ConfigEntry<int> DebuffStacksPerItemStack;

        public void Awake()
        {
            const string chronobaubleSection = "Chronobauble";

            SlowScalingCoefficient = Config.AddSetting<float>(
                new ConfigDefinition(chronobaubleSection, nameof(SlowScalingCoefficient)),
                0.05f,
                new ConfigDescription(
                    "The scaling coefficient for how much each stack of slow will slow enemies (higher is slower)",
                    new AcceptableValueRange<float>(0.00f, 0.20f)
                    ));

            DebuffStacksPerItemStack = Config.AddSetting<int>(
                new ConfigDefinition(chronobaubleSection, nameof(DebuffStacksPerItemStack)),
                5,
                new ConfigDescription(
                    "The maximum number of slow debuff stacks you can give for every chronobauble stack you have",
                    new AcceptableValueRange<int>(0, 20)
                    ));

            On.RoR2.CharacterBody.SetBuffCount += CharacterBody_SetBuffCount;
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            var c = new ILCursor(il);
            ILLabel label = il.DefineLabel();

            // Add logic only add Slow60 buff if you have stacks to permit doing it
            c.GotoNext(
                x => x.MatchLdloc(1),
                x => x.MatchLdcI4(26),
                x => x.MatchLdcR4(2)
                );
            c.Emit(OpCodes.Ldloc_S, (byte)10); //Number of Chronobaubles on attacker
            c.Emit(OpCodes.Ldloc_1); //Victim CharacterBody
            c.EmitDelegate<Func<int, CharacterBody, bool>>((chronobaubleCount, victim) =>
            {
                if (DebuffStacksPerItemStack.Value > 0 &&
                    victim.GetBuffCount(BuffIndex.Slow60) >= DebuffStacksPerItemStack.Value * chronobaubleCount)
                {
                    return false;
                }                
                return true;               
            });
            c.Emit(OpCodes.Brfalse, label); //If delegate returns false, break and do not add buff
            c.GotoNext(x => x.MatchLdloc(0));
            c.MarkLabel(label);
        }   

        private void CharacterBody_SetBuffCount(On.RoR2.CharacterBody.orig_SetBuffCount orig, CharacterBody self, BuffIndex buffType, int newCount)
        {
            if (buffType == BuffIndex.Slow60)
            {
                BuffCatalog.GetBuffDef(buffType).canStack = true;
            }
            orig(self, buffType, newCount);
        }

        private void CharacterBody_RecalculateStats(MonoMod.Cil.ILContext il)
        {
            var c = new ILCursor(il);

            // Multiply movement speed by coefficient based on number of chronobaubles you have
            // Note: this is in addition to the Slow60 movement speed buff
            // IL_0603
            c.GotoNext(x => x.MatchCallvirt<CharacterBody>("set_moveSpeed"));
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
            {
                if (cb.HasBuff(BuffIndex.Slow60))
                {
                    return GetDiminishingReturns(cb.GetBuffCount(BuffIndex.Slow60));
                }
                return 1.0f;
            });
            c.Emit(OpCodes.Mul);


            // Multiply attack speed by coefficient based on number of chronobaubles you have
            // IL_07b5
            c.GotoNext(x => x.MatchCallvirt<CharacterBody>("set_attackSpeed"));
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
            {
                if (cb.HasBuff(BuffIndex.Slow60))
                {
                    return GetDiminishingReturns(cb.GetBuffCount(BuffIndex.Slow60));
                }
                return 1.0f;
            });
            c.Emit(OpCodes.Mul);
        }

        private float GetDiminishingReturns(int count)
        {
            return 1.0f / (count * SlowScalingCoefficient.Value + 1);
        }
    }
}
