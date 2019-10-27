namespace PimPamPum
{
    public class WaitForResponse : WaitForDecision
    {
        public Card ResponseCard { get; private set; }

        public WaitForResponse() : base(Decision.TakeHit) { }

        public override void MakeDecisionCard(Decision decision, Card card)
        {
            base.MakeDecisionCard(decision, card);
            ResponseCard = card;
        }
    }
}