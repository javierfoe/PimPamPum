using System.Collections;

namespace PimPamPum
{
    public class MadamYto : PlayerController
    {

        protected override IEnumerator UsedBeerTrigger(int player)
        {
            yield return PimPamPumEvent(this + " draws a card for the beer just used.");
            Draw(1);
        }

    }
}

