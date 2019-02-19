﻿
using System.Collections;

namespace Bang
{
    public class BelleStar : PlayerController
    {

        protected override IEnumerator OnStartTurn()
        {
            GameController.Instance.EnableOthersProperties(PlayerNumber,false);
            yield return base.OnStartTurn();
        }

        public override void ForceEndTurn()
        {
            GameController.Instance.EnableOthersProperties(PlayerNumber, true);
            base.ForceEndTurn();
        }

        protected override string Character()
        {
            return "Belle Star";
        }

    }
}