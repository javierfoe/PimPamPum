using System.Collections;

namespace PimPamPum
{
    public class YoulGrinner : PlayerController
    {
        protected override IEnumerator DrawPhase1()
        {
            yield return GameController.YoulGrinnerSkill(PlayerNumber);
            yield return base.DrawPhase1();
        }
    }
}