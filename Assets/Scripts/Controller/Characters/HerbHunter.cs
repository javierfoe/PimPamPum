
using System.Collections.Generic;

namespace Bang
{
    public class HerbHunter : PlayerController
    {

        public override bool CheckDeath(List<Card> list)
        {
            Draw(2);
            return false;
        }

        protected override string Character()
        {
            return "Herb Hunter";
        }

    }
}
