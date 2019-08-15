namespace PimPamPum
{
    public class WaitForResponse : WaitForDecision
    {
        public Card ResponseCard { get; private set; }

        public WaitForResponse() : base(Decision.TakeHit) { }

        public override void MakeDecision(Decision decision, Card card)
        {
            base.MakeDecision(decision, card);
            ResponseCard = card;
        }
    }
}