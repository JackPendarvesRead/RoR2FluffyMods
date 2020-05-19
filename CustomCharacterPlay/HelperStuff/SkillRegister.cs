using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CustomCharacterPlay.HelperStuff
{
    public class SkillRegister
    {
        private readonly SkillLocator locator;

        private readonly Dictionary<SkillType, string> dic;

        public SkillRegister(SkillLocator locator)
        {
            this.locator = locator;
            dic = new Dictionary<SkillType, string>
            {
                { SkillType.Primary, locator.primary.skillDef.activationStateMachineName },
                { SkillType.Secondary, locator.secondary.skillDef.activationStateMachineName },
                { SkillType.Special, locator.special.skillDef.activationStateMachineName },
                { SkillType.Utility, locator.utility.skillDef.activationStateMachineName }
            };
        }

        public void RegisterSkills()
        {
            var skills = Assembly.GetCallingAssembly().DefinedTypes
                .Where(x => typeof(ICustomSkill).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => (ICustomSkill)x.Instantiate());

            foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
            {
                RegisterSkillType(skills.Where(x => x.SkillType == skillType).ToList());
            }
        }

        private void RegisterSkillType(List<ICustomSkill> skills)
        {
            if(skills.Count > 0)
            {
                SkillFamily family = GetSkillFamily(skills);

                foreach (var skill in skills)
                {
                    LoadoutAPI.AddSkill(skill.GetType());
                }
                foreach (var variant in family.variants)
                {
                    LoadoutAPI.AddSkillDef(variant.skillDef);
                }
                LoadoutAPI.AddSkillFamily(family);

                switch (skills.First().SkillType)
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

        private SkillFamily GetSkillFamily(List<ICustomSkill> skills)
        {
            var family = SkillFamily.CreateInstance<SkillFamily>();
            family.defaultVariantIndex = 0;
            family.variants = GetVarients(skills);
            ((ScriptableObject)family).name = skills.First().SkillType.ToString();
            return family;
        }

        private SkillFamily.Variant[] GetVarients(List<ICustomSkill> skills)
        {
            var variants = new SkillFamily.Variant[skills.Count];
            for (var i = 0; i < skills.Count(); i++)
            {
                var activationStateMachineName = dic[skills[i].SkillType] ?? "Unknown";
                variants[i] = new SkillFamily.Variant()
                {
                    skillDef = skills[i].GetSkillDefinition(activationStateMachineName),
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(skills[i].SkillType.ToString(), false)
                };
            }
            return variants;
        }
    }
}
