using System.Collections;

namespace PimPamPum
{
    public class BillNoface : PlayerController
    {

        protected override IEnumerator DrawPhase1()
        {
            int drawncards = 1 + MaxHP - HP;
            yield return PimPamPumEvent(this + " draws a total of " + drawncards + " cards.");
            Draw(drawncards);
        }

        protected override string Character()
        {
            return "Bill Noface";
        }

    }
}
