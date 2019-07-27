using RoR2;
using UnityEngine;

namespace EntityStates.Skills
{
    class FirePistol : BaseState
    {
        public static float baseDuration = 2f;
        public static float recoilAmplitude = 1f;
        public static GameObject effectPrefab;
        public static GameObject hitEffectPrefab;
        public static GameObject tracerEffectPrefab;
        public static float damageCoefficient;
        public static float force;
        public static float minSpread;
        public static float maxSpread;
        public static string firePistolSoundString;
        private Ray aimRay;
        private bool hasFiredSecondBullet;
        private float duration;

        private void FireBullet(string targetMuzzle)
        {
            if ((bool)((Object)FirePistol.effectPrefab))
                EffectManager.instance.SimpleMuzzleFlash(FirePistol.effectPrefab, this.gameObject, targetMuzzle, false);
            this.AddRecoil(-0.4f * FirePistol.recoilAmplitude, -0.8f * FirePistol.recoilAmplitude, -0.3f * FirePistol.recoilAmplitude, 0.3f * FirePistol.recoilAmplitude);
            if (!this.isAuthority)
                return;
            new BulletAttack()
            {
                owner = this.gameObject,
                weapon = this.gameObject,
                origin = this.aimRay.origin,
                aimVector = this.aimRay.direction,
                minSpread = FirePistol.minSpread,
                maxSpread = FirePistol.maxSpread,
                damage = (FirePistol.damageCoefficient * this.damageStat),
                force = FirePistol.force,
                tracerEffectPrefab = FirePistol.tracerEffectPrefab,
                muzzleName = targetMuzzle,
                hitEffectPrefab = FirePistol.hitEffectPrefab,
                isCrit = Util.CheckRoll(this.critStat, this.characterBody.master)
            }.Fire();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = FirePistol.baseDuration / this.attackSpeedStat;
            this.aimRay = this.GetAimRay();
            this.StartAimMode(this.aimRay, 2f, false);
            this.FireBullet("MuzzleLeft");
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((double)this.fixedAge < (double)this.duration || !this.isAuthority)
                return;
            this.outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
