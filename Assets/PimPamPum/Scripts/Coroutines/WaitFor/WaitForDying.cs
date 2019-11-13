using System;

namespace PimPamPum
{
    public class WaitForDying : WaitForDecision
    {
        private Func<bool> dying;

        public override bool MoveNext()
        {
            bool res = dying();
            Finished(res);
            return res;
        }

        protected override void Finished()
        {
            WaitForController.StopMainCorutine();
        }

        public WaitForDying(PlayerController pc) : base(pc)
        {
            dying = () => base.MoveNext() && pc.IsDying;
            WaitForController.DyingCorutine = this;
        }
    }
}
