using System.Collections;

namespace PimPamPum
{
    public class TerenKill : PlayerController
    {

        protected override IEnumerator DieTrigger(int killer)
        {
            DrawEffectCoroutine drawEffectCoroutine = new DrawEffectCoroutine(this);
            yield return drawEffectCoroutine;
            Card drawEffectCard = drawEffectCoroutine.DrawEffectCard;
            if (drawEffectCard.Suit != Suit.Spades)
            {
                yield return PimPamPumEvent(this + " draws! to stay alive: " + drawEffectCard);
                HP = 1;
                Draw(1);
            }
            else
            {
                yield return PimPamPumEvent(this + " draws! and dies: " + drawEffectCard);
                yield return base.DieTrigger(killer);
            }
        }

    }
}