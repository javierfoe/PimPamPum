
using System.Collections.Generic;

namespace Bang
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