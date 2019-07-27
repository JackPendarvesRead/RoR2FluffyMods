using RoR2;

namespace EntityStates.BirdShark
{
    public class BirdSharkSpecial : BaseState
    {
        private float totalDuration = 10f;

        public override void OnEnter()
        {
            base.OnEnter();
            Chat.AddMessage("HAHAHAHAHAHAHAH!!");
        }
        public override void OnExit()
        {
            SmallHop(characterMotor, 100f);
            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= this.totalDuration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}
