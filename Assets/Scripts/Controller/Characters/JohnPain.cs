using System.Collections;

namespace Bang
{
    public class JohnPain : PlayerController
    {

        public override IEnumerator DrawEffectTrigger(Card c)
        {
            if(!IsDead && hand.Count < 6)
            {
                GameController.PickedCard = true;
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