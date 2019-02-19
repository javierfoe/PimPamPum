using System.Collections;

namespace Bang
{
    public class JohnPain : PlayerController
    {

        protected override IEnumerator DrawEffectTrigger(Card c)
        {
            if(hand.Count < 6)
            {
                GameController.Instance.PickedCard = true;
                yield return BangEvent(this + " adds the draw! effect card: " + c);
                AddCard(c);
            }
        }

        protected override string Character()
        {
            return "John Pain";
        }

    }
}