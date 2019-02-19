using System.Collections;

namespace Bang
{
    public class ColoradoBill : PlayerController
    {

        protected override IEnumerator ShotBangTrigger(int target)
        {
            yield return GameController.Instance.DrawEffect(PlayerNumber);
            if (GameController.Instance.DrawnCard.Suit == Suit.Spades)
            {
                yield return BangEvent(this + " has shot an undodgeable Bang! " + GameController.Instance.DrawnCard);
                yield return GameController.Instance.HitPlayer(PlayerNumber, target);
            }
            else
            {
                yield return BangEvent(this + " has shot an standard Bang! " + GameController.Instance.DrawnCard);
                yield return base.ShotBangTrigger(target);
            }
        }

        protected override string Character()
        {
            return "Colorado Bill";
        }

    }
}