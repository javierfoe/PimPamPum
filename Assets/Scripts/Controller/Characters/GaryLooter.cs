using System.Collections;

namespace Bang
{
    public class GaryLooter : PlayerController
    {

        public override IEnumerator EndTurnDiscard(Card c)
        {
            GameController.PickedCard = true;
            yield return BangEvent(this + " adds the discarded card to his hand: " + c);
            AddCard(c);
        }

        protected override string Character()
        {
            return "Gary Looter";
        }

    }
}