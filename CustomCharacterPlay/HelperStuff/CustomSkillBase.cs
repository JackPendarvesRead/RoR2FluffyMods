using EntityStates;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomCharacterPlay.HelperStuff
{
    public abstract class CustomSkill : BaseSkillState, ICustomSkill
    {
        public SkillType SkillType { get; }
        public abstract SkillDef GetSkillDefinition();
        BaseSkillState EntityState { get; set; }
    }
}
