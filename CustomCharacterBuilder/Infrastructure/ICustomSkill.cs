using RoR2.Skills;

namespace CustomCharacterBuilder.Infrastructure
{
    public interface ICustomSkill
    {
        SkillType SkillType { get; }
        SkillDef GetSkillDefinition(string activationStateMachineName);
    }
}
