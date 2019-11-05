using System.Collections;

namespace PimPamPum
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

    }
}