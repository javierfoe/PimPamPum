
using System.Collections;

namespace Bang
{
    public class ColoradoBill : PlayerController
    {

        public override IEnumerator ShotBang(int target, Card card)
        {
            bangsUsed++;
            yield return GameController.DrawEffect(PlayerNumber);
            if (GameController.DrawnCard.Suit == Suit.Spades)
            {
                yield return BangEvent(this + " has shot an undodgeable Bang! " + GameController.DrawnCard);
                GameController.HitPlayer(PlayerNumber, target);
            }
            else
            {
                yield return BangEvent(this + " has shot an standard Bang! " + GameController.DrawnCard);
                yield return GameController.Bang(PlayerNumber, target, card, MissesToDodge);
            }
        }

        protected override string Character()
        {
            return "Colorado Bill";
        }

    }
}