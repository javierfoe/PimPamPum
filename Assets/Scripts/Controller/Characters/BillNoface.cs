using System.Collections;

namespace Bang
{
    public class BillNoface : PlayerController
    {

        protected override IEnumerator DrawPhase1()
        {
            Draw(1 + MaxHP - HP);
            yield return null;
        }

        protected override string Character()
        {
            return "Bill Noface";
        }

    }
}
