using CustomCharacterBuilder.Infrastructure;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomCharacterBuilder.Logic
{
    public static class SkillRegister
    {
        public static void RegisterSkills(IEnumerable<ICustomSkill> skills, SkillLocator locator)
        {
            foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
            {
                RegisterSkillType(skills.Where(x => x.SkillType == skillType).ToList(), locator, skillType);
            }
        }

        private static void RegisterSkillType(List<ICustomSkill> skills, SkillLocator locator, SkillType skillType)
        {
            if(skills.Count > 0)
            {
                SkillFamily family = GetSkillFamily(skills);
                RegisterSkillsLoadoutAPI(skills);
                RegisterSkillFamilyAndVariantsLoadoutAPI(family);
                SetSkillFamilyField(skillType, locator, family);
            }
            else
            {
                Debug.LogWarning($"No skills found of type '{skillType}' for this custom character.");
            }
        }

        private static SkillFamily GetSkillFamily(List<ICustomSkill> skills)
        {
            var family = SkillFamily.CreateInstance<SkillFamily>();
            family.defaultVariantIndex = 0;
            family.variants = GetVarients(skills);
            ((ScriptableObject)family).name = skills.First().SkillType.ToString();
            return family;
        }

        private static SkillFamily.Variant[] GetVarients(List<ICustomSkill> skills)
        {
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
            return variants;
        }

        private static void RegisterSkillsLoadoutAPI(List<ICustomSkill> skills)
        {
            foreach (var skill in skills)
            {
                LoadoutAPI.AddSkill(skill.GetType());
            }
        }

        private static void RegisterSkillFamilyAndVariantsLoadoutAPI(SkillFamily family)
        {
            foreach (var variant in family.variants)
            {
                LoadoutAPI.AddSkillDef(variant.skillDef);
            }
            LoadoutAPI.AddSkillFamily(family);
        }

        private static void SetSkillFamilyField(SkillType skillType, SkillLocator locator, SkillFamily family)
        {
            switch (skillType)
            {
                case SkillType.Primary:
                    locator.primary.SetFieldValue("_skillFamily", family);
                    break;
                case SkillType.Secondary:
                    locator.secondary.SetFieldValue("_skillFamily", family);
                    break;
                case SkillType.Special:
                    locator.special.SetFieldValue("_skillFamily", family);
                    break;
                case SkillType.Utility:
                    locator.utility.SetFieldValue("_skillFamily", family);
                    break;
                case SkillType.Passive:
                    locator.passiveSkill.SetFieldValue("_skillFamily", family);
                    break;
            }
        }
    }
}
