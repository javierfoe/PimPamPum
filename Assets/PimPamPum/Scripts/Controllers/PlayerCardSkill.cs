using System.Collections;

namespace PimPamPum
{
    public abstract class PlayerCardSkill : PlayerController
    {
        protected bool usedSkillCard;
        private bool activeSkill;

        protected abstract bool CanUseSkill();

        protected override void EnablePhase2Cards()
        {
            OriginalHand();
            base.EnablePhase2Cards();
            activeSkill = false;
            bool skill = CanUseSkill();
            EnableSkill(skill);
            SetSkill(skill);
        }

        public override void UseSkill()
        {
            SetSkill(activeSkill);
            if (activeSkill)
            {
                EnablePhase2Cards();
            }
            else
            {
                EnableSkillCards();
                activeSkill = true;
            }
        }

        protected virtual void EnableSkillCards()
        {
            EnableCards<ConvertedCard>();
            SetSkill(false);
        }

        protected override IEnumerator OnStartTurn()
        {
            usedSkillCard = false;
            yield return base.OnStartTurn();
        }

        public override void UsedSkillCard()
        {
            usedSkillCard = true;
        }
    }
}