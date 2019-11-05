
namespace PimPamPum
{
    public class ApacheKid : PlayerController
    {

        public override bool Immune(Card c)
        {
            return c.IsSuit(Suit.Diamonds);
        }

    }
}
