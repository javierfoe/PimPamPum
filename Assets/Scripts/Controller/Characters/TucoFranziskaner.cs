
using System.Collections;

namespace PimPamPum
{
    public class TucoFranziskaner : PlayerController
    {

        protected override IEnumerator DrawPhase1()
        {
            Draw(2);
            if (!HasProperties && HasColt45)
            {
                yield return PimPamPumEvent(this + " draws 2 extra cards. He has no blue cards on play in front of him");
                Draw(2);
            }
        }

        protected override string Character()
        {
            return "Tuco Franziskaner";
        }

    }
}
