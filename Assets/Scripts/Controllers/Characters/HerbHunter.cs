
using System.Collections.Generic;

namespace PimPamPum
{
    public class HerbHunter : PlayerController
    {

        public override bool CheckDeath(List<Card> list)
        {
            Draw(2);
            return false;
        }

    }
}
