using System.Collections;

namespace PimPamPum
{
    public class JohnPain : PlayerController
    {

        protected override IEnumerator DrawEffectTrigger(Card c)
        {
            if(hand.Count < 6)
            {
                GameController.Instance.PickedCard = true;
                yield return PimPamPumEvent(this + " adds the draw! effect card: " + c);
                AddCard(c);
            }
        }

    }
}