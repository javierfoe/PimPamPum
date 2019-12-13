using System.Collections;

namespace PimPamPum
{
    public class ChuckWengam : PlayerController
    {
        protected override void EnablePhase2Cards()
        {
            base.EnablePhase2Cards();
            EnableSkill();
        }

        private void EnableSkill()
        {
            bool value = hp > 1;
            EnableSkill(value);
            SetSkill(value);
        }

        protected override void UseSkill()
        {
            StartCoroutine(ChuckWenganCoroutine());
        }

        private IEnumerator ChuckWenganCoroutine()
        {
            DisableCards();
            EnableConfirmButton(true);
            EnableCancelButton(true);
            WaitForDecision decision = new WaitForDecision(this, Decision.Cancel);
            yield return decision;
            if (decision.Decision == Decision.Confirm)
            {
                hp--;
                Draw(2);
            }
            EnableCancelButton(false);
            EnableConfirmButton(false);
            EnableCardsPlay();
        }
    }
}