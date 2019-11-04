using System;

namespace PimPamPum
{
    public class WaitForDying : WaitForDecision
    {
        private Func<bool> dying;

        public override bool MoveNext()
        {
            bool res = dying();
            if (!res && mainCorutine != null && mainCorutine != this) mainCorutine.StopCorutine();
            return res;
        }

        public WaitForDying(PlayerController pc) : base()
        {
            dying = () => base.MoveNext() && pc.IsDying;
            dyingCorutine = this;
        }
    }
}
