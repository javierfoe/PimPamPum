﻿using System.Collections;

namespace PimPamPum
{
    public class JohnnyKisch : PlayerController
    {

        public override IEnumerator Equip<T>(Property p)
        {
            yield return GameController.DiscardCopiesOf<T>(PlayerNumber, p);
        }

    }
}