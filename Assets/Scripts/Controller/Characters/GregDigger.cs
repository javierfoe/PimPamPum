
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

        protected override string Character()
        {
            return "Greg Digger";
        }

    }
}