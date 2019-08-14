using System.Collections;

namespace PimPamPum
{
    public class HenryBlock : PlayerController
    {

        public override IEnumerator StolenBy(int thief)
        {
            if (thief != PlayerNumber && thief != PimPamPumConstants.NoOne)
            {
                yield return GameController.Instance.PimPamPum(PimPamPumConstants.NoOne, thief);
            }
            yield return base.StolenBy(thief);
        }

    }
}
