using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates
{
    public class SecondarySkill : BaseState
    {
        private readonly GameObject prefab = (GameObject)Resources.Load(@"prefabs/effects/tracers/tracersmokeline/tracermagelightninglaser");
        private float distance = 1;

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Secondary Enter");
            var aimRay = this.GetAimRay();
            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.teamMaskFilter = TeamMask.AllExcept(this.GetTeam());
            bullseyeSearch.maxAngleFilter = 45;
            bullseyeSearch.maxDistanceFilter = 40f;
            bullseyeSearch.searchOrigin = aimRay.origin;
            bullseyeSearch.searchDirection = aimRay.direction;
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
            bullseyeSearch.filterByLoS = false;
            bullseyeSearch.RefreshCandidates();
            //.Where<HurtBox>(new Func<HurtBox, bool>(Util.IsValid)).Distinct<HurtBox>((IEqualityComparer<HurtBox>)new HurtBox.EntityEqualityComparer()
            foreach (HurtBox hurtBox in bullseyeSearch.GetResults().Where<HurtBox>(new Func<HurtBox, bool>(Util.IsValid)).Distinct<HurtBox>((IEqualityComparer<HurtBox>)new HurtBox.EntityEqualityComparer()))
            {
                Debug.Log($"Target found - {hurtBox.healthComponent.name} - {hurtBox.transform.position.ToString()}");

                Vector3 distanceToTarget = hurtBox.transform.position - aimRay.origin;
                Vector3 normal = distanceToTarget / distanceToTarget.magnitude;

                var body = hurtBox.healthComponent.body;
                body.RecalculateStats();

                var coeff = Trajectory.CalculateInitialYSpeedForHeight(Mathf.Abs(distance - normal.magnitude), body.acceleration);
                Debug.Log($"Coefficient = {coeff}");

                var final = normal * coeff * Mathf.Sign(distance - normal.magnitude);
                Debug.Log("Final = " + final.ToString());
                hurtBox.healthComponent.TakeDamageForce(final, true, true);
                hurtBox.healthComponent.TakeDamage(new DamageInfo()
                {
                    attacker = this.gameObject,
                    damage = 0.0f,
                    position = hurtBox.transform.position,
                    procCoefficient = 0.0f
                });
            }
        }

        public override void OnExit()
        {
            Debug.Log("Secondary Exit");
            base.OnExit();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
