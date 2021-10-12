﻿using System.Collections;

namespace PimPamPum
{
    public class ColoradoBill : PlayerController
    {

        protected override IEnumerator ShootPimPamPumTrigger(int target)
        {
            DrawEffectCoroutine drawEffectCoroutine = new DrawEffectCoroutine(this);
            yield return drawEffectCoroutine;
            Card drawEffectCard = drawEffectCoroutine.DrawEffectCard;
            if (drawEffectCard.Suit == Suit.Spades)
            {
                yield return PimPamPumEvent(this + " has shot an undodgeable PimPamPum! " + drawEffectCard);
                yield return GameController.HitPlayer(PlayerNumber, target);
            }
            else
            {
                yield return PimPamPumEvent(this + " has shot an standard PimPamPum! " + drawEffectCard);
                yield return base.ShootPimPamPumTrigger(target);
            }
        }

    }
}