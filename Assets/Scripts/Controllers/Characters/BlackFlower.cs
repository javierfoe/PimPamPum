namespace PimPamPum
{
    public class BlackFlower : PlayerCardSkill
    {
        protected override bool CanUseSkill()
        {
            if (usedSkillCard) return false;
            bool clubs = false;
            for(int i = 0; i < Hand.Count && !clubs; i++)
            {
                clubs = Hand[i].Suit == Suit.Clubs;
            }
            return clubs;
        }

        protected override void EnableSkillCards()
        {
            ConvertHandTo<ExtraPimPamPum>(Suit.Clubs);
            base.EnableSkillCards();
        }
    }
}