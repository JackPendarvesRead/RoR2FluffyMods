using CustomCharacterBuilder.Infrastructure;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomCharacterBuilder.Logic
{
    public class SkillRegister
    {
        private readonly SkillLocator locator;

        public SkillRegister(SkillLocator locator)
        {
            this.locator = locator;
        }

        public void RegisterSkills()
        {
            var skills = Assembly.GetCallingAssembly().DefinedTypes
                .Where(x => typeof(ICustomSkill).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => (ICustomSkill)x.Instantiate());

            foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
            {
                Debug.Log("Skill type = " + skillType.ToString());
                RegisterSkillType(skills.Where(x => x.SkillType == skillType).ToList(), skillType);
            }
        }

        private void RegisterSkillType(List<ICustomSkill> skills, SkillType skillType)
        {
            if(skills.Count > 0)
            {
                SkillFamily family = GetSkillFamily(skills);
                RegisterSkillsAndFamilyWithLoadoutAPI(skills, family);
                SetSkillFamilyField(skillType, family);
            }
            else
            {
                Debug.LogWarning($"No skills found of type '{skillType}' for this custom character.");
            }
        }

        private SkillFamily GetSkillFamily(List<ICustomSkill> skills)
        {
            Debug.Log("FamilyStart");

            var family = SkillFamily.CreateInstance<SkillFamily>();
            family.defaultVariantIndex = 0;
            family.variants = GetVarients(skills);
            ((ScriptableObject)family).name = skills.First().SkillType.ToString();
            Debug.Log("FamilyEnd");
            return family;
        }

        private SkillFamily.Variant[] GetVarients(List<ICustomSkill> skills)
        {
            Debug.Log("VariantStart");
            var variants = new SkillFamily.Variant[skills.Count];
            for (var i = 0; i < skills.Count(); i++)
            {
                var activationStateMachineName = "Weapon";
                variants[i] = new SkillFamily.Variant()
                {
                    skillDef = skills[i].GetSkillDefinition(activationStateMachineName),
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(skills[i].SkillType.ToString(), false)
                };
            }
            Debug.Log("VariantEnd");
            return variants;
        }

        private void RegisterSkillsAndFamilyWithLoadoutAPI(List<ICustomSkill> skills, SkillFamily family)
        {
            foreach (var skill in skills)
            {
                LoadoutAPI.AddSkill(skill.GetType());
            }
            foreach (var variant in family.variants)
            {
                LoadoutAPI.AddSkillDef(variant.skillDef);
            }
            LoadoutAPI.AddSkillFamily(family);
        }

        private void SetSkillFamilyField(SkillType skillType, SkillFamily family)
        {
            switch (skillType)
            {
                case SkillType.Primary:
                    locator.primary.SetFieldValue<SkillFamily>("_skillFamily", family);
                    break;
                case SkillType.Secondary:
                    locator.secondary.SetFieldValue<SkillFamily>("_skillFamily", family);
                    break;
                case SkillType.Special:
                    locator.special.SetFieldValue<SkillFamily>("_skillFamily", family);
                    break;
                case SkillType.Utility:
                    locator.utility.SetFieldValue<SkillFamily>("_skillFamily", family);
                    break;
                case SkillType.Passive:
                    locator.passiveSkill.SetFieldValue<SkillFamily>("_skillFamily", family);
                    break;
            }
        }
    }
}
