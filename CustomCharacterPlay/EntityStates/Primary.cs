using BepInEx.Logging;
using CustomCharacterPlay.HelperStuff;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace EntityStates.MyCustomCharacter
{
    public class Primary : BaseState, ICustomSkill
    {
        public SkillType SkillType => SkillType.Primary;
        public SkillDef GetSkillDefinition()
        {
            SkillDef primaryDef = SkillDef.CreateInstance<SkillDef>();
            primaryDef.activationState = new EntityStates.SerializableEntityStateType(typeof(Primary));
            var field2 = typeof(EntityStates.SerializableEntityStateType)?.GetField("_typeName", BindingFlags.NonPublic | BindingFlags.Instance);
            field2?.SetValue(primaryDef.activationState, typeof(Primary)?.AssemblyQualifiedName);
            primaryDef.baseRechargeInterval = 1f;
            primaryDef.baseMaxStock = 5;
            primaryDef.rechargeStock = 1;
            primaryDef.skillName = "MySkillName";
            primaryDef.skillNameToken = "My Name Token";
            primaryDef.skillDescriptionToken = "This is the description of the primary skill.";
            primaryDef.activationStateMachineName = "MachineName I don't know?!";
            primaryDef.isBullets = false;
            primaryDef.beginSkillCooldownOnSkillEnd = true;
            primaryDef.interruptPriority = EntityStates.InterruptPriority.Any;
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
