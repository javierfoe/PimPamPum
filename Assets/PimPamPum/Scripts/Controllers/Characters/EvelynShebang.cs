using System.Collections;

namespace PimPamPum
{
    public class EvelynShebang : PlayerController
    {
        protected override IEnumerator DrawPhase1()
        {
            yield return new EvelynShebangSkillCoroutine(this, Phase1CardsDrawn);
        }
    }
}