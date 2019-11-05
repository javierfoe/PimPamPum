using System.Collections;

namespace PimPamPum
{
    public class DoubleCard : Card
    {
        private Card first, second;

        public DoubleCard(Card one, Card two)
        {
            first = one;
            second = two;
        }

        public override bool IsSuit(Suit suit)
        {
            return first.IsSuit(suit) && second.IsSuit(suit);
        }

        public override IEnumerator CardUsed(PlayerController pc) { yield return null; }
    }
}