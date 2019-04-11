
using System.Collections.Generic;

namespace PimPamPum
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

    }
}
