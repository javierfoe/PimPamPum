using System.Collections;

namespace PimPamPum
{
    public class GaryLooter : PlayerController
    {

        protected override IEnumerator EndTurnDiscardTrigger(Card c)
        {
            if (!IsDead)
            {
                GameController.Instance.PickedCard = true;
                yield return PimPamPumEvent(this + " adds the discarded card to his hand: " + c);
                AddCard(c);
            }
        }

    }
}