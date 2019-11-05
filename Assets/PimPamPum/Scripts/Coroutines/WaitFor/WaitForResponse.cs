namespace PimPamPum
{
    public class WaitForCardResponse : WaitForDecision
    {
        public Card ResponseCard { get; private set; }

        public WaitForCardResponse() : base(Decision.TakeHit) { }

        public override void MakeDecisionCard(Decision decision, Card card)
        {
            base.MakeDecisionCard(decision, card);
            ResponseCard = card;
        }
    }
}