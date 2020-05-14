using CustomCharacterPlay.HelperStuff;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EntityStates.MyCustomCharacter
{
    public class Secondary : BaseSkillState, ICustomSkill
    {
        public SkillType SkillType => SkillType.Secondary;
        public SkillDef GetSkillDefinition()
        {
            SkillDef def = SkillDef.CreateInstance<SkillDef>();
            def.activationState = new EntityStates.SerializableEntityStateType(typeof(Primary));
            var field2 = typeof(EntityStates.SerializableEntityStateType)?.GetField("_typeName", BindingFlags.NonPublic | BindingFlags.Instance);
            field2?.SetValue(def.activationState, typeof(EntityStates.MyCustomCharacter.Primary)?.AssemblyQualifiedName);
            def.baseRechargeInterval = 1f;
            def.baseMaxStock = 1;
            def.rechargeStock = 1;
            def.skillName = "MySkillNameSecond";
            def.skillNameToken = "My Name Token Second";
            def.skillDescriptionToken = "This is the description of the Second skill.";
            def.activationStateMachineName = "MachineName I don't know?!";
            def.isBullets = false;
            def.beginSkillCooldownOnSkillEnd = true;
            def.interruptPriority = EntityStates.InterruptPriority.Any;
            def.isCombatSkill = true;
            def.noSprint = false;
            def.canceledFromSprinting = false;
            def.mustKeyPress = true;
            def.requiredStock = 1;
            def.stockToConsume = 0;
            //primaryDef.icon = Assets.SephIcon;
            return def;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Chat.AddMessage("Second on enter");
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}