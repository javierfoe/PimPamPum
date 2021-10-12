using System.Collections;

namespace PimPamPum
{
    public class BelleStar : PlayerController
    {

        protected override IEnumerator OnStartTurn()
        {
            GameController.EnableOthersProperties(PlayerNumber,false);
            yield return base.OnStartTurn();
        }

        public override void ForceEndTurn()
        {
            GameController.EnableOthersProperties(PlayerNumber, true);
            base.ForceEndTurn();
        }

    }
}