using System.Collections;

namespace PimPamPum
{
    public class ColoradoBill : PlayerController
    {

        protected override IEnumerator ShotPimPamPumTrigger(int target)
        {
            yield return GameController.Instance.DrawEffect(PlayerNumber);
            if (GameController.Instance.DrawnCard.Suit == Suit.Spades)
            {
                yield return PimPamPumEvent(this + " has shot an undodgeable PimPamPum! " + GameController.Instance.DrawnCard);
                yield return GameController.Instance.HitPlayer(PlayerNumber, target);
            }
            else
            {
                yield return PimPamPumEvent(this + " has shot an standard PimPamPum! " + GameController.Instance.DrawnCard);
                yield return base.ShotPimPamPumTrigger(target);
            }
        }

    }
}