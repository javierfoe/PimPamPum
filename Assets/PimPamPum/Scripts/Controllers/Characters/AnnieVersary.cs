namespace PimPamPum
{
    public class AnnieVersary : PlayerCardSkill
    {
        protected override bool CanUseSkill()
        {
            return HasOtherThanHand<PimPamPum>() && CanShoot;
        }

        private bool HasOtherThanHand<T>() where T : Card, new()
        {
            bool res = false;
            int length = Hand.Count;
            for (int i = 0; i < length && !res; i++)
            {
                res = !Hand[i].Is<T>();
            }
            return res;
        }

        protected override void EnableSkillCards()
        {
            ConvertHandTo<PimPamPum>();
            base.EnableSkillCards();
        }

        protected override void EnablePimPamPumCardsForReaction()
        {
            ConvertHandTo<PimPamPum>();
            base.EnablePimPamPumCardsForReaction();
        }
    }
}