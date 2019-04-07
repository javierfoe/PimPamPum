using System.Collections;

namespace PimPamPum
{
    public class MadamYto : PlayerController
    {

        protected override IEnumerator UsedBeerTrigger()
        {
            if (!IsDead)
            {
                yield return PimPamPumEvent(this + " draws a card for the beer just used.");
                Draw(1);
            }
        }

        protected override string Character()
        {
            return "Madam Yto";
        }

    }
}

