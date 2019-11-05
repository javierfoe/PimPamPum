namespace PimPamPum
{
    public class FlintWestwood : PlayerCardSkill
    {
        protected override bool CanUseSkill()
        {
            return !usedSkillCard;
        }

        protected override void EnableSkillCards()
        {
            ConvertHandTo<TradeTwoForOne>();
            base.EnableSkillCards();
        }

        public override void UsedSkillCard()
        {
            usedSkillCard = true;
        }
    }
}