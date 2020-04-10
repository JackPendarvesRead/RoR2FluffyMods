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
    [R2APISubmoduleDependency(nameof(ItemAPI))]
    public class ChronobaubleFix : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "ChronobaubleFix";
        private const string pluginVersion = "2.0.2";

        private static ConfigEntry<float> SlowScalingCoefficient;
        private static ConfigEntry<int> DebuffStacksPerItemStack;
        private static ConfigEntry<bool> ChronobaubleFixEnabled;
        private readonly float buffDuration = 5f;

        private CustomBuff chronoFixBuff;

        public void Awake()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }

            #region ConfigSetup
            const string chronobaubleSection = "Chronobauble";

            SlowScalingCoefficient = Config.Bind<float>(
                new ConfigDefinition(chronobaubleSection, nameof(SlowScalingCoefficient)),
                0.05f,
                new ConfigDescription(
                    "The scaling coefficient for how much each stack of slow will slow enemies (higher is slower)",
                    new AcceptableValueRange<float>(0.00f, 0.20f)
                    ));

            DebuffStacksPerItemStack = Config.Bind<int>(
                new ConfigDefinition(chronobaubleSection, nameof(DebuffStacksPerItemStack)),
                3,
                new ConfigDescription(
                    "The maximum number of slow debuff stacks you can give for every chronobauble stack you have",
                    new AcceptableValueRange<int>(1, 20)
                    ));

            ChronobaubleFixEnabled = Config.Bind<bool>(
               new ConfigDefinition(chronobaubleSection, nameof(ChronobaubleFixEnabled)),
               true,
               new ConfigDescription(
                   "Turn the mod on or off"
                   ));
            #endregion

            RegisterCustomBuff();
            IL.RoR2.GlobalEventManager.OnHitEnemy += OnHitEnemyAddCustomBuff;
            IL.RoR2.CharacterBody.RecalculateStats += SetMovementAndAttackSpeed;
            
            //RoR2.SceneDirector.onPostPopulateSceneServer += SubscribeToHooks;          
        }

        private void RegisterCustomBuff()
        {
            string name = "ChronobaubleFixBuff";
            chronoFixBuff = new CustomBuff(name, new BuffDef
            {
                buffColor = new Color(0.6784314f, 0.6117647f, 0.4117647f),
                canStack = true,
                iconPath = "Textures/BuffIcons/texBuffSlow50Icon",
                isDebuff = true,
                name = name
            });
            ItemAPI.Add(chronoFixBuff);
        }

        private bool ModIsActive
        {
            get
            {
                if (ChronobaubleFixEnabled.Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #region SubscribeFuckery
        private bool hooksCurrentlyEnabled = false;

        private void SubscribeToHooks(SceneDirector obj)
        {
            if (RoR2.Run.instance)
            {
                if (ChronobaubleFixEnabled.Value)
                {
                    if (hooksCurrentlyEnabled)
                    {
                        if (NetworkUser.readOnlyInstancesList.Count > 1)
                        {
                            Unsubscribe();
                        }
                    }
                    else
                    {
                        Subscribe();
                    }
                }
                else
                {
                    if (hooksCurrentlyEnabled)
                    {
                        Unsubscribe();
                    }
                }
            }
        }

        private void Unsubscribe()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy -= OnHitEnemyAddCustomBuff;
            IL.RoR2.CharacterBody.RecalculateStats -= SetMovementAndAttackSpeed;
            hooksCurrentlyEnabled = false;
            Debug.Log("Unsubscibing hooks. Currently this mod will only work for single player games.");
        }

        private void Subscribe()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += OnHitEnemyAddCustomBuff;
            IL.RoR2.CharacterBody.RecalculateStats += SetMovementAndAttackSpeed;
            hooksCurrentlyEnabled = true;
            Debug.Log("Subscribing to hooks");
        }
        #endregion

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
                    var buffIndex = chronoFixBuff.BuffDef.buffIndex;
                    var victimCurrentBuffCount = victimBody.GetBuffCount(buffIndex);
                    var maximumBuffCount = DebuffStacksPerItemStack.Value * attackerChronobaubleCount;

                    Logger.LogInfo($"Attacker = {attackerBody.name}, Victim = {victimBody.name}, BuffCount = {victimCurrentBuffCount}, Max = {maximumBuffCount}");
                    Logger.LogInfo($"BUFFINDEX = {buffIndex}");
                    if (victimCurrentBuffCount < maximumBuffCount)
                    {
                        victimBody.AddTimedBuff(buffIndex, buffDuration);
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
                var buffindex = chronoFixBuff.BuffDef.buffIndex;
                if (cb.HasBuff(buffindex))
                {
                    return GetDiminishingReturns(cb.GetBuffCount(buffindex));
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
                var buffindex = chronoFixBuff.BuffDef.buffIndex;
                if (cb.HasBuff(buffindex))
                {
                    return GetDiminishingReturns(cb.GetBuffCount(buffindex));
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
