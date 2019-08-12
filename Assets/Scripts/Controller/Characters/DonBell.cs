using System.Collections;

namespace PimPamPum
{

    public class DonBell : PlayerController
    {

        private bool extraTurn;

        protected override IEnumerator OnStartTurn()
        {
            extraTurn = false;
            return base.OnStartTurn();
        }

        protected override void WillinglyEndTurn()
        {
            if(extraTurn)
            {
                extraTurn = false;
                base.WillinglyEndTurn();
                return;
            }
            StartCoroutine(ExtraTurnCheck());
        }

        private IEnumerator ExtraTurnCheck()
        {
            DisableCards();
            DrawEffectCoroutine drawEffectCoroutine = new DrawEffectCoroutine(this, GameController.Instance.DecisionTime);
            yield return drawEffectCoroutine;
            Card drawEffectCard = drawEffectCoroutine.DrawEffectCard;
            extraTurn = drawEffectCard.IsRed;
            yield return PimPamPumEvent(this + " has drawn: " + drawEffectCard + (extraTurn ? " he gets another turn. " : " he ends the turn normally."));

            if (extraTurn)
            {
                yield return base.OnStartTurn();
            }
            else
            {
                base.WillinglyEndTurn();
            }
        }

    }
}