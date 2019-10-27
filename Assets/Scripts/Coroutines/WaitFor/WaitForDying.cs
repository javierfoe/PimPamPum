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
            dyingCorutine = this;
        }

        public override void MakeDecisionCard(Decision decision, Card card = null)
        {
            WaitFor waitFor;
            if ((waitFor = Current as WaitFor) != null)
            {
                waitFor.MakeDecisionCard(decision, card);
            }
            if (decision == Decision.Die)
            {
                base.MakeDecisionCard(decision, card);
            }
        }

        public override void MakeDecisionPhaseOne(Decision phaseOneOption, int player, Drop dropEnum, int card)
        {
            WaitFor waitFor;
            if ((waitFor = Current as WaitFor) != null)
            {
                waitFor.MakeDecisionPhaseOne(phaseOneOption, player, dropEnum, card);
            }
            else
            {
                base.MakeDecisionPhaseOne(phaseOneOption, player, dropEnum, card);
            }
        }

        public override void MakeDecisionCardIndex(int card)
        {
            WaitFor waitFor;
            if ((waitFor = Current as WaitFor) != null)
            {
                waitFor.MakeDecisionCardIndex(card);
            }
            else
            {
                base.MakeDecisionCardIndex(card);
            }
        }

    }
}