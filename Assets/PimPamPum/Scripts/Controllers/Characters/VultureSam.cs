
using System.Collections.Generic;

namespace PimPamPum
{
    public class VultureSam : PlayerController
    {

        public override bool CheckDeathTrigger(List<Card> list)
        {
            foreach(Card c in list)
            {
                AddCard(c);
            }
            return true;
        }

    }
}
