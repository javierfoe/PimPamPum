using System;

namespace PimPamPum
{
    public class WaitForDying : WaitForDecision
    {
        private Func<bool> dying;

        public override bool MoveNext()
        {
            bool res = dying();
            if (!res) WaitForController.StopMainCorutine();
            return res;
        }

        public WaitForDying(PlayerController pc) : base(pc)
        {
            dying = () => base.MoveNext() && pc.IsDying;
            WaitForController.DyingCorutine = this;
        }
    }
}
