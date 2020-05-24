using CustomCharacterPlay.HelperStuff;
using EntityStates;
using EntityStates.Treebot.Weapon;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CustomCharacterPlay.Skills
{
    public class Utility : BaseState, ICustomSkill
    {
        private readonly float liftVelocity = 5f;
        private readonly float idealDistanceToPlaceTargets = 10f;
        private readonly float maxDistance = 100f;
        private readonly float fieldOfView = 10f;

        public SkillType SkillType => SkillType.Utility;

        public SkillDef GetSkillDefinition(string activationStateMachineName)
        {
            SkillDef def = SkillDef.CreateInstance<SkillDef>();
            def.activationState = new EntityStates.SerializableEntityStateType(typeof(Utility));
            def.baseRechargeInterval = 3f;
            def.baseMaxStock = 1;
            def.rechargeStock = 1;
            def.skillName = "UtilityName";
            def.skillNameToken = "My Name Token Utitlity";
            def.skillDescriptionToken = "This is the description of the Utility skill.";
            def.activationStateMachineName = activationStateMachineName;
            def.isBullets = false;
            def.beginSkillCooldownOnSkillEnd = true;
            def.interruptPriority = EntityStates.InterruptPriority.Skill;
            def.isCombatSkill = true;
            def.noSprint = false;
            def.canceledFromSprinting = false;
            def.mustKeyPress = true;
            def.requiredStock = 1;
            def.stockToConsume = 1;
            //primaryDef.icon = Assets.SephIcon;
            return def;
        }

        public override void OnEnter()
        {
            Debug.Log("UtilityONEnter");
            base.OnEnter();
            var aimRay = this.GetAimRay();
            BullseyeSearch bullseyeSearch = new BullseyeSearch
            {
                teamMaskFilter = TeamMask.all,
                maxAngleFilter = fieldOfView * 0.5f,
                maxDistanceFilter = maxDistance,
                searchOrigin = aimRay.origin,
                searchDirection = aimRay.direction,
                sortMode = BullseyeSearch.SortMode.Distance,
                filterByLoS = false
            };
            bullseyeSearch.RefreshCandidates();
            bullseyeSearch.FilterOutGameObject(this.gameObject);

            //var hurtBoxes = bullseyeSearch.GetResults()
            //    .Where(new Func<HurtBox, bool>(Util.IsValid))
            //    .Distinct(new HurtBox.EntityEqualityComparer());
            Debug.Log("Getting Results");
            var hurtBoxes = bullseyeSearch.GetResults();
            Debug.Log("Got results");
            foreach (HurtBox hurtBox in hurtBoxes)
            {
                Debug.Log("Found hurtbox = " + hurtBox.name);
            }
        }
    }
}
