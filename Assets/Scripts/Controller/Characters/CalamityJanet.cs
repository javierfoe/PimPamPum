
namespace PimPamPum
{

    public class CalamityJanet : PlayerCardConverter
    {

        protected override void EnablePhase2Cards()
        {
            ConvertHandCardTo<Missed, PimPamPum>();
            base.EnablePhase2Cards();
        }

        protected override void EnablePimPamPumCardsForReaction()
        {
            ConvertHandCardTo<Missed, PimPamPum>();
            base.EnablePimPamPumCardsForReaction();
        }

        protected override void EnablePimPamPumReaction()
        {
            ConvertHandCardTo<PimPamPum, Missed>();
            base.EnablePimPamPumReaction();
        }

        protected override string Character()
        {
            return "Calamity Janet";
        }
    }
}