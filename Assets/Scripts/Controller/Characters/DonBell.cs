﻿using System.Collections;

namespace Bang
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
            yield return GameController.DrawEffect(PlayerNumber);
            extraTurn = GameController.DrawnCard.IsRed;
            yield return BangEvent(this + " has drawn: " + GameController.DrawnCard + (extraTurn ? " he gets another turn. " : " he ends the turn normally."));

            if (extraTurn)
            {
                yield return OnStartTurn();
            }
            else
            {
                base.WillinglyEndTurn();
            }
        }

        protected override string Character()
        {
            return "Don Bell";
        }
    }
}