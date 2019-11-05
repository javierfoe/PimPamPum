
using System.Collections.Generic;

namespace PimPamPum
{
    public class GregDigger : PlayerController
    {

        public override bool CheckDeathTrigger(List<Card> list)
        {
            Heal(2);
            return false;
        }

    }
}