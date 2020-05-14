using EntityStates;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomCharacterPlay.HelperStuff
{
    public interface ICustomSkill
    {
        SkillType SkillType { get; }
        SkillDef GetSkillDefinition();
    }
}
