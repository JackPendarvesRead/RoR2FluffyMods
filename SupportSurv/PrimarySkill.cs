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
    public class PrimarySkill : BaseState
    {
        private readonly GameObject prefab = (GameObject)Resources.Load(@"prefabs/effects/tracers/tracersmokeline/tracermagelightninglaser");

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Primary Enter");
            var aimRay = this.GetAimRay();
            var attack = new BulletAttack
            {
                aimVector = aimRay.direction,
                origin = aimRay.origin,
                damage = 50,
                bulletCount = 1,
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.Generic,
                falloffModel = BulletAttack.FalloffModel.None,
                force = 0,
                isCrit = false,
                HitEffectNormal = true,
                maxDistance = 100,
                maxSpread = 5,
                minSpread = 3,
                tracerEffectPrefab = prefab
            };
            attack.Fire();            
        }

        public override void OnExit()
        {
            Debug.Log("Primary Exit");
            base.OnExit();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
