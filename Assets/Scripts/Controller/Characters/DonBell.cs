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
            yield return GameController.Instance.DrawEffect(PlayerNumber);
            extraTurn = GameController.Instance.DrawnCard.IsRed;
            yield return PimPamPumEvent(this + " has drawn: " + GameController.Instance.DrawnCard + (extraTurn ? " he gets another turn. " : " he ends the turn normally."));

            if (extraTurn)
            {
                yield return OnStartTurn();
            }
            else
            {
                base.WillinglyEndTurn();
            }
        }

    }
}