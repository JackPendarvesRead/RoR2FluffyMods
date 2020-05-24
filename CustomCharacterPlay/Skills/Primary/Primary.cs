using CustomCharacterBuilder.Infrastructure;
using EntityStates;
using RoR2;
using RoR2.Skills;

namespace CustomCharacterPlay.Skills.Primary
{
    public class Primary : BaseSkillState, ICustomSkill
    {
        public SkillType SkillType => SkillType.Primary;
        public SkillDef GetSkillDefinition(string activationStateMachineName)
        {
            SkillDef primaryDef = SkillDef.CreateInstance<SkillDef>();
            primaryDef.activationState = new EntityStates.SerializableEntityStateType(typeof(Primary));
            primaryDef.baseRechargeInterval = 1f;
            primaryDef.baseMaxStock = 5;
            primaryDef.rechargeStock = 1;
            primaryDef.skillName = "MySkillName";
            primaryDef.skillNameToken = "My Name Token";
            primaryDef.skillDescriptionToken = "This is the description of the primary skill.";
            primaryDef.activationStateMachineName = activationStateMachineName;
            primaryDef.isBullets = false;
            primaryDef.beginSkillCooldownOnSkillEnd = true;
            primaryDef.interruptPriority = EntityStates.InterruptPriority.Skill;
            primaryDef.isCombatSkill = true;
            primaryDef.noSprint = false;
            primaryDef.canceledFromSprinting = false;
            primaryDef.mustKeyPress = true;
            primaryDef.requiredStock = 1;
            primaryDef.stockToConsume = 1;       
            return primaryDef;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            UnityEngine.Debug.Log("BLABLAKBLFALFSKALFAK");
            Chat.AddMessage("Primary on enter");
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }
        private float duration = 0.5f;

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
