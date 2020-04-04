using BepInEx;
using EntityStates;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using R2API.Utils;

namespace SupportSurv
{
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class SupporTSurv : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "SupportSurv";
        private const string pluginVersion = "1.0.0";

        public void Awake()
        {
            GameObject bodyPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/BanditBody");
            var display = bodyPrefab.GetComponent<RoR2.ModelLocator>().modelTransform.gameObject;
            SurvivorDef survDef = new SurvivorDef
            {
                bodyPrefab = bodyPrefab,
                name = "Bloop",
                descriptionToken = "Bloop",
                displayPrefab = display,
                primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f)
            };
            R2API.SurvivorAPI.AddSurvivor(survDef);

            CharacterBody body = bodyPrefab.GetComponent<CharacterBody>();

            SetStateOnHurt hurtState = bodyPrefab.AddOrGetComponent<SetStateOnHurt>();
            hurtState.canBeFrozen = true;
            hurtState.canBeHitStunned = false;
            hurtState.canBeStunned = false;
            hurtState.hitThreshold = 5f;
            var entityStateMachines = bodyPrefab.GetComponentsInChildren<EntityStateMachine>();
            var machines = new List<EntityStateMachine>();
            foreach(var esm in entityStateMachines)
            {
                if (esm.customName == "Body")
                {
                    hurtState.targetStateMachine = esm;
                }
                else
                {
                    machines.Add(esm);
                }
            }
            hurtState.idleStateMachine = machines.ToArray();

            SkillLocator skillLocator = bodyPrefab.GetComponent<SkillLocator>();
            SkillFamily SkillFamily = bodyPrefab.GetComponent<SkillFamily>();
            SkillFamily primarySkillFamily = skillLocator.primary.skillFamily;
            SkillFamily secondarySkillFamily = skillLocator.secondary.skillFamily;
            SkillFamily utilitySkillFamily = skillLocator.utility.skillFamily;
            SkillFamily specialSkillFamily = skillLocator.special.skillFamily;
            SkillDef primary = primarySkillFamily.variants[primarySkillFamily.defaultVariantIndex].skillDef;
            SkillDef secondary = secondarySkillFamily.variants[secondarySkillFamily.defaultVariantIndex].skillDef;
            SkillDef utility = utilitySkillFamily.variants[utilitySkillFamily.defaultVariantIndex].skillDef;
            SkillDef special = specialSkillFamily.variants[specialSkillFamily.defaultVariantIndex].skillDef;

            primary.activationState = new SerializableEntityStateType(typeof(PrimarySkill));
            primary.baseMaxStock = 1;
            primary.baseRechargeInterval = 1;
            primary.requiredStock = 1;
            primary.stockToConsume = 1;
            primary.shootDelay = 1;
            primary.rechargeStock = 1;
            primary.requiredStock = 1;            
            primary.canceledFromSprinting = true;
            primary.fullRestockOnAssign = true;
            primary.beginSkillCooldownOnSkillEnd = false;
            primary.isBullets = false;
            primary.isCombatSkill = true;
            primary.mustKeyPress = false;
            primary.interruptPriority = InterruptPriority.Skill;
            primary.skillName = "Primary Skill";
            primary.skillNameToken = "Primary Skill";
            primary.skillDescriptionToken = "Shoot gun for <color=#E5C962>damage.</color>";


            secondary.activationState = new SerializableEntityStateType(typeof(SecondarySkill));
            secondary.baseMaxStock = 1;
            secondary.baseRechargeInterval = 1;
            secondary.requiredStock = 1;
            secondary.stockToConsume = 1;
            secondary.shootDelay = 1;
            secondary.rechargeStock = 1;
            secondary.requiredStock = 1;
            secondary.canceledFromSprinting = true;
            secondary.fullRestockOnAssign = true;
            secondary.beginSkillCooldownOnSkillEnd = false;
            secondary.isBullets = false;
            secondary.isCombatSkill = true;
            secondary.mustKeyPress = true;
            secondary.interruptPriority = InterruptPriority.Skill;
            secondary.skillName = "Primary Skill";
            secondary.skillNameToken = "Primary Skill";
            secondary.skillDescriptionToken = "Shoot gun for <color=#E5C962>damage.</color>";

        }
    }
}
