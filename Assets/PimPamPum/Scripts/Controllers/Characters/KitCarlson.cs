using System.Collections;

namespace PimPamPum
{

    public class KitCarlson : PlayerController
    {

        protected override IEnumerator DrawPhase1()
        {
            yield return GameController.ChooseCardToPutOnDeckTop(PlayerNumber);
        }

    }
}