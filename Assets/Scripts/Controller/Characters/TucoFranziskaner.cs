
using System.Collections;

namespace Bang
{
    public class TucoFranziskaner : PlayerController
    {

        protected override IEnumerator DrawPhase1()
        {
            if(!HasProperties && HasColt45)
            {
                Draw(2);
            }
            yield return base.DrawPhase1();
        }

        protected override string Character()
        {
            return "Tuco Franziskaner";
        }

    }
}
