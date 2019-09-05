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
            bool value = HP > 1;
            EnableSkill(value);
            SetSkillStatus(value);
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
            WaitForDecision decision = new WaitForDecision(Decision.Cancel);
            yield return decision;
            if (decision.Decision == Decision.Confirm)
            {
                HP--;
                Draw(2);
            }
            EnableCancelButton(false);
            EnableConfirmButton(false);
            EnableCardsPlay();
        }

        public override void DisableCards()
        {
            base.DisableCards();
            EnableSkill(false);
        }
    }
}