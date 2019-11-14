using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace SupportSurv
{
    public class PrimarySkill : BaseState
    {
        private Ray aimRay;
        private Transform modelTransform;
        private string targetMuzzle;
        private GameObject projectilePrefab;
        private float spreadBloomValue;
        private float damageCoefficient;

        public void Awake()
        {
            var x = R2API.AssetAPI.BodyCatalog;
            
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.aimRay = this.GetAimRay();
            if ((bool)((Object)this.modelTransform))
            {
                ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
                if ((bool)((Object)component))
                {
                    Transform child = component.FindChild(targetMuzzle);
                    if ((bool)((Object)child))
                        this.aimRay.origin = child.position;
                }
            }
            if (this.isAuthority)
            {
                Vector3 forward = Vector3.ProjectOnPlane(this.aimRay.direction, Vector3.up);
                var info = new FireProjectileInfo
                {
                    projectilePrefab = projectilePrefab,
                    crit = false,
                    damage = 1,
                    damageColorIndex = DamageColorIndex.Default,
                    force = 1,
                    owner = gameObject,
                    position = transform.position,
                    procChainMask = new ProcChainMask(),
                    rotation = Util.QuaternionSafeLookRotation(forward),
                    target = null,
                    useFuseOverride = false,
                    useSpeedOverride = false
                };
                ProjectileManager.instance.FireProjectile(info);
            }
            this.characterBody.AddSpreadBloom(spreadBloomValue);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
