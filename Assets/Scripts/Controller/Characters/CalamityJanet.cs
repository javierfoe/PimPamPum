
namespace Bang
{

    public class CalamityJanet : PlayerController
    {

        protected override void EnablePhase2Cards()
        {
            ConvertHandCardTo<Missed, Bang>();
            base.EnablePhase2Cards();
        }

        protected override void EnableBangCardsForReaction()
        {
            ConvertHandCardTo<Missed, Bang>();
            base.EnableBangCardsForReaction();
        }

        protected override void EnableBangReaction()
        {
            ConvertHandCardTo<Bang, Missed>();
            base.EnableBangReaction();
        }

        protected override string Character()
        {
            return "Calamity Janet";
        }
    }
}