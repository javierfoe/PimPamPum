namespace PimPamPum
{
    public abstract class WaitFor : Enumerator
    {
        protected static WaitFor mainCorutine, dyingCorutine;

        public static void MakeDecision(int card)
        {
            mainCorutine.MakeDecisionCardIndex(card);
        }

        public static void MakeDecision(Decision decision, Card card = null)
        {
            WaitFor waitFor = mainCorutine;
            if (decision == Decision.Die) waitFor = dyingCorutine;
            waitFor.MakeDecisionCard(decision, card);
        }

        public static void MakeDecision(Decision phaseOne, int player, Drop dropEnum, int card)
        {
            mainCorutine.MakeDecisionPhaseOne(phaseOne, player, dropEnum, card);
        }

        public virtual void StopCorutine() { }
        public virtual void MakeDecisionCardIndex(int card) { }
        public virtual void MakeDecisionCard(Decision decision, Card card) { }
        public virtual void MakeDecisionPhaseOne(Decision phaseOneOption, int player, Drop dropEnum, int card) { }
    }
}