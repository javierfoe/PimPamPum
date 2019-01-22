using System.Collections;

namespace Bang
{
    public class GaryLooter : PlayerController
    {

        protected override IEnumerator EndTurnDiscardTrigger(Card c)
        {
            if (!IsDead)
            {
                GameController.PickedCard = true;
                yield return BangEvent(this + " adds the discarded card to his hand: " + c);
                AddCard(c);
            }
        }

        protected override string Character()
        {
            return "Gary Looter";
        }

    }
}