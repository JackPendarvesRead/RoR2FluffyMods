using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;

namespace ChronobaubleFix
{
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    [R2APISubmoduleDependency(nameof(BuffAPI))]
    public class ChronobaubleFix : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "ChronobaubleFix";
        private const string pluginVersion = "3.0.0";

        private ConfigEntry<float> DebuffDuration;
        private ConfigEntry<int> DebuffStacksPerItemStack;
        private ConfigEntry<bool> ChronobaubleFixEnabled;
        private ConfigEntry<float> IncreasedDebuffDurationPerStack;
        private ConfigEntry<float> SlowScalingCoefficient;

        //private CustomBuff chronoFixBuff;
        private BuffIndex CustomChronobaubleBuffIndex;

        public void Awake()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }

            RegisterConfiguration();
            RegisterCustomBuff();
            IL.RoR2.GlobalEventManager.OnHitEnemy += OnHitEnemyAddCustomBuff;
            IL.RoR2.CharacterBody.RecalculateStats += SetMovementAndAttackSpeed;
        }

        private void RegisterConfiguration()
        {
            const string chronobaubleSection = "ChronobaubleFix";
            const string durationSection = "Debuff Duration";
            const string scalingSection = "Scaling";

            ChronobaubleFixEnabled = Config.Bind<bool>(
               new ConfigDefinition(chronobaubleSection, nameof(ChronobaubleFixEnabled)),
               true,
               new ConfigDescription(
                   "Turn the mod on or off"
                   ));

            DebuffDuration = Config.Bind<float>(
                new ConfigDefinition(durationSection, nameof(DebuffDuration)),
                2f,
                new ConfigDescription(
                    "The time (in seconds) a debuff will last on an enemy. Default = 2 seconds",
                    new AcceptableValueRange<float>(0.0f, 10f)
                    ));

            DebuffStacksPerItemStack = Config.Bind<int>(
               new ConfigDefinition(scalingSection, nameof(DebuffStacksPerItemStack)),
               3,
               new ConfigDescription(
                   "The maximum number of slow debuff stacks you can give for every chronobauble stack you have",
                   new AcceptableValueRange<int>(1, 20)
                   ));

            IncreasedDebuffDurationPerStack = Config.Bind<float>(
                durationSection,
                nameof(IncreasedDebuffDurationPerStack),
                0f,
                new ConfigDescription(
                   "Increases duration of buff by this amount for each chronobauble stack on attacker over 1",
                   new AcceptableValueRange<float>(0.00f, 0.50f)
                   ));

            SlowScalingCoefficient = Config.Bind<float>(
                new ConfigDefinition(scalingSection, nameof(SlowScalingCoefficient)),
                0.035f,
                new ConfigDescription(
                    "The scaling coefficient for how much each stack of slow will slow enemies (higher is slower)",
                    new AcceptableValueRange<float>(0.00f, 0.50f)
                    ));
        }

        private void RegisterCustomBuff()
        {
            var buffDef = new CustomBuff("ChronobaubleFixBuff", "Textures/BuffIcons/texBuffSlow50Icon", new Color(0.6784314f, 0.6117647f, 0.4117647f), true, true);
            CustomChronobaubleBuffIndex = BuffAPI.Add(buffDef);
        }

        private bool ModIsActive
        {
            get
            {
                if (ChronobaubleFixEnabled.Value &&
                    NetworkUser.readOnlyInstancesList.Count < 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private int victimBodyIndex;
        private void OnHitEnemyAddCustomBuff(ILContext il)
        {
            var c = new ILCursor(il);
            ILLabel label = il.DefineLabel();

            Mono.Cecil.FieldReference fr1;
            c.GotoNext(
                x => x.MatchLdarg(2),
                x => x.MatchCallvirt<GameObject>("GetComponent"),
                x => x.MatchStloc(out victimBodyIndex));

            c.GotoNext(
                x => x.MatchLdloc(victimBodyIndex),
                x => x.MatchLdcI4(26),
                x => x.MatchLdcR4(2)
                );

            c.Emit(OpCodes.Ldarg_1); //Arg1 = DamageInfo
            c.Emit(OpCodes.Ldarg_2); //Arg2 = VictimGameObj
            c.EmitDelegate<Func<DamageInfo, GameObject, bool>>((damageInfo, victimGameObject) =>
            {  
                if (ModIsActive)
                {
                    var attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    var victimBody = victimGameObject.GetComponent<CharacterBody>();

                    var attackerChronobaubleCount = attackerBody.inventory.GetItemCount(ItemIndex.SlowOnHit);
                    var victimCurrentBuffCount = victimBody.GetBuffCount(CustomChronobaubleBuffIndex);
                    var maximumBuffCount = DebuffStacksPerItemStack.Value * attackerChronobaubleCount;

                    if (victimCurrentBuffCount < maximumBuffCount)
                    {                   
                        float debuffDuration = DebuffDuration.Value + IncreasedDebuffDurationPerStack.Value * (attackerChronobaubleCount - 1);
                        victimBody.AddTimedBuff(CustomChronobaubleBuffIndex, debuffDuration);
                    }
                    return false;
                }
                else
                {
                    return true;
                }                              
            });
            c.Emit(OpCodes.Brfalse, label); //If delegate returns false, break and do not add buff
            c.GotoNext(x => x.MatchLdloc(0));
            c.MarkLabel(label);
        }   

        private void SetMovementAndAttackSpeed(ILContext il)
        {
            var c = new ILCursor(il);

            // Multiply movement speed by coefficient based on number of chronobaubles you have
            // Note: this is in addition to the Slow60 movement speed buff
            // IL_0603
            c.GotoNext(x => x.MatchCallvirt<CharacterBody>("set_moveSpeed"));
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
            {
                if (cb.HasBuff(CustomChronobaubleBuffIndex))
                {
                    return GetDiminishingReturns(cb.GetBuffCount(CustomChronobaubleBuffIndex));
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
                if (cb.HasBuff(CustomChronobaubleBuffIndex))
                {
                    return GetDiminishingReturns(cb.GetBuffCount(CustomChronobaubleBuffIndex));
                }
                return 1.0f;
            });
            c.Emit(OpCodes.Mul);
        }

        private float GetDiminishingReturns(int itemCount)
        {
            return 1.0f / (itemCount * SlowScalingCoefficient.Value + 1);
        }
    }
}
