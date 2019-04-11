
using System.Collections.Generic;

namespace PimPamPum
{
    public class GregDigger : PlayerController
    {

        public override bool CheckDeath(List<Card> list)
        {
            Heal(2);
            return false;
        }

    }
}