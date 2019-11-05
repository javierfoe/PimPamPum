using System.Collections;

namespace PimPamPum
{
    public class ExtraPimPamPum : PimPamPum
    {
        protected override IEnumerator Shoot(PlayerController pc, int target)
        {
            yield return pc.ShootPimPamPum(target);
        }

        public override IEnumerator CardUsed(PlayerController pc)
        {
            yield return GameController.Instance.UsedCard<ExtraPimPamPum>(pc);
        }

        public override string ToString()
        {
            return "ExtraPimPamPum";
        }
    }
}