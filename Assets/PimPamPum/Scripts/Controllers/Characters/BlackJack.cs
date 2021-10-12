using System.Collections;

namespace PimPamPum
{
    public class BlackJack : PlayerController
    {

        protected override IEnumerator DrawPhase1()
        {
            Draw();
            Card c = GameController.DrawCard();
            AddCard(c);
            bool anotherCard = c.IsRed;
            yield return PimPamPumEvent(this + " has drawn:" + c + (anotherCard ? " he draws another card." : " he doesn't draw extra."));
            if (anotherCard)
            {
                Draw();
            }
        }

    }
}
