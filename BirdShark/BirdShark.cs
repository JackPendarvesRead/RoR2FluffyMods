using BepInEx;
using EntityStates;
using R2API;
using RoR2;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BirdShark
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.FluffyMods.BirdShark", "BirdShark", "1.0.0")]
    public class BirdShark : BaseUnityPlugin
    {
        private string prefabString = PrefabStrings.BanditBody;

        public void Awake()
        {
            SurvivorAPI.SurvivorCatalogReady += delegate (object s, EventArgs e)
            {
                SurvivorDef survivor = new SurvivorDef
                {
                    bodyPrefab = BodyCatalog.FindBodyPrefab(prefabString),
                    descriptionToken = "This is the description for the birdshark.",
                    displayPrefab = Resources.Load<GameObject>(prefabString),
                    primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f),
                    unlockableName = "",
                    survivorIndex = SurvivorIndex.Count + 1
                };
                SurvivorAPI.SurvivorDefinitions.Insert(SurvivorAPI.SurvivorDefinitions.Count, survivor);
            };

            On.RoR2.Stage.Start += Stage_Start;
        }

        private void Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);
            try
            {
                var users = NetworkUser.readOnlyInstancesList
                .Where(u => u.GetCurrentBody().name.ToLower().StartsWith(prefabString.ToLower()))
                .Select(u => u.GetCurrentBody());
                foreach (var user in users)
                {
                    SetupSkills(user);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);                
            }
        }

        private void SetupSkills(CharacterBody body)
        {
            var sl = body.GetComponent<SkillLocator>();
            GameObject gameObject = BodyCatalog.FindBodyPrefab(prefabString);
            SetupSkill<EntityStates.Skills.FirePistol>(sl.primary);
            //SetupSkill<EntityStates.LemurianMonster.Bite>(sl.secondary);
            //SetupSkill<EntityStates.Assassin.Weapon.SlashCombo>(sl.special);
            //SetupSkill<EntityStates.Commando.CommandoWeapon.CastSmokescreen>(sl.utility);
        }

        private void SetupSkill<T>(GenericSkill skill)
        {
            //skill.baseMaxStock = 1;
            //skill.baseRechargeInterval = 3;
            //skill.beginSkillCooldownOnSkillEnd = true;
            //skill.canceledFromSprinting = true;
            //skill.isCombatSkill = true;
            //skill.requiredStock = 1;
            //skill.skillName = "NAME";
            //skill.skillNameToken = "NAMETOKEN";
            //skill.stockToConsume = 1;
            //skill.skillDescriptionToken = "THIS IS THE FUCKING DESCRIPTION BITCH!";

            skill.activationState = new EntityStates.SerializableEntityStateType(typeof(T));
            object box = skill.activationState;
            var field = typeof(EntityStates.SerializableEntityStateType)?.GetField("_typeName", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(box, typeof(T)?.AssemblyQualifiedName);
            skill.activationState = (EntityStates.SerializableEntityStateType)box;
        }
        private void SetupSkill(GenericSkill skill, string entityString)
        {
            //skill.baseMaxStock = 1;
            //skill.baseRechargeInterval = 3;
            //skill.beginSkillCooldownOnSkillEnd = true;
            //skill.canceledFromSprinting = true;
            //skill.isCombatSkill = true;
            //skill.requiredStock = 1;
            //skill.skillName = "NAME";
            //skill.skillNameToken = "NAMETOKEN";
            //skill.stockToConsume = 1;
            //skill.skillDescriptionToken = "THIS IS THE FUCKING DESCRIPTION BITCH!";

            skill.activationState = new EntityStates.SerializableEntityStateType(entityString);            
            object box = skill.activationState;
            var field = typeof(EntityStates.SerializableEntityStateType)?.GetField("_typeName", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(box, skill.activationState.GetType()?.AssemblyQualifiedName);
            skill.activationState = (EntityStates.SerializableEntityStateType)box;
        }
    }
}