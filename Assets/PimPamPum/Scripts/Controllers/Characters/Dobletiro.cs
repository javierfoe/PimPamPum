using System.Collections;

namespace PimPamPum
{
    public class Dobletiro : PlayerCardSkill
    {
        protected override bool CanUseSkill()
        {
            if (usedSkillCard) return false;
            return HasHand<PimPamPum>();
        }

        protected override void EnableSkillCards()
        {
            ConvertHandCardTo<PimPamPum, Gatling>();
            base.EnableSkillCards();
        }

        public override void UsedSkillCard()
        {
            usedSkillCard = true;
        }
    }
}