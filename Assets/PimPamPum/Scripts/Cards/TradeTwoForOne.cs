using System.Collections;

namespace PimPamPum
{
    public class TradeTwoForOne : Card
    {

        public override void BeginCardDrag(PlayerController pc)
        {
            base.BeginCardDrag(pc);
            pc.TargetOthersWithHand();
        }

        public override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return null;
            pc.TradeTwoForOne(player);
        }

        public override IEnumerator CardUsed(PlayerController pc)
        {
            yield return GameController.UsedCard<TradeTwoForOne>(pc);
        }

        public override string ToString()
        {
            return "TradeTwoForOne";
        }
    }
}