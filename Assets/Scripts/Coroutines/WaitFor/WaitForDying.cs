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

        public override void MakeDecision(Decision decision, Card card = null)
        {
            WaitFor waitFor;
            if ((waitFor = Current as WaitFor) != null)
            {
                waitFor.MakeDecision(decision, card);
            }
            if (decision == Decision.Die)
            {
                base.MakeDecision(decision, card);
            }
        }

        public override void MakeDecision(Decision phaseOneOption, int player, Drop dropEnum, int card)
        {
            WaitFor waitFor;
            if ((waitFor = Current as WaitFor) != null)
            {
                waitFor.MakeDecision(phaseOneOption, player, dropEnum, card);
            }
            else
            {
                base.MakeDecision(phaseOneOption, player, dropEnum, card);
            }
        }

        public override void MakeDecision(int card)
        {
            WaitFor waitFor;
            if ((waitFor = Current as WaitFor) != null)
            {
                waitFor.MakeDecision(card);
            }
            else
            {
                base.MakeDecision(card);
            }
        }

    }
}