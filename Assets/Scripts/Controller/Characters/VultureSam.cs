
using System.Collections.Generic;

namespace Bang
{
    public class VultureSam : PlayerController
    {

        public override bool CheckDeath(List<Card> list)
        {
            foreach(Card c in list)
            {
                AddCard(c);
            }
            return true;
        }

        protected override string Character()
        {
            return "Vulture Sam";
        }

    }
}
