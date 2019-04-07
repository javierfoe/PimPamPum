
namespace PimPamPum
{
    public class ApacheKid : PlayerController
    {

        public override bool Immune(Card c)
        {
            return c.Suit == Suit.Diamonds;
        }

        protected override string Character()
        {
            return "Apache Kid";
        }

    }
}
