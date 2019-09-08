namespace PimPamPum
{
    public class LeeVanKliff : PlayerCardSkill
    {
        private Card lastUsedCard;

        protected override bool CanUseSkill()
        {
            return lastUsedCard != null && HasHand<PimPamPum>();
        }

        protected override void UseCardState(int index, int player, Drop drop, int cardIndex)
        {
            if (draggedCard.IsBrown) lastUsedCard = draggedCard;
            base.UseCardState(index, player, drop, cardIndex);
        }

        protected override void EnableSkillCards()
        {
            ConvertHandFrom<PimPamPum>(lastUsedCard);
            base.EnableSkillCards();
        }

        public override void UsedSkillCard()
        {
            base.UsedSkillCard();
            lastUsedCard = null;
        }
    }
}