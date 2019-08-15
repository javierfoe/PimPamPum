using System;

namespace PimPamPum
{
    public class WaitForDying : WaitForDecision
    {

        private Func<bool> dying;

        public override bool MoveNext()
        {
            return dying();
        }

        public WaitForDying(PlayerController pc) : base()
        {
            dying = () => base.MoveNext() && pc.IsDying;
        }

    }
}