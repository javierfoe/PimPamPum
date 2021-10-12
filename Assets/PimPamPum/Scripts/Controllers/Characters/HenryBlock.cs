﻿using System.Collections;

namespace PimPamPum
{
    public class HenryBlock : PlayerController
    {

        public override IEnumerator StolenBy(int thief)
        {
            if (thief != PlayerNumber && thief != PimPamPumConstants.NoOne)
            {
                yield return GameController.PimPamPum(PimPamPumConstants.NoOne, thief);
            }
        }

    }
}
