
using System.Collections;

namespace Bang
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

        protected override string Character()
        {
            return "Belle Star";
        }

    }
}