using System.Collections;
namespace PimPamPum
{
    public class PimPamPum : Card
    {
        public override void BeginCardDrag(PlayerController pc)
        {
            base.BeginCardDrag(pc);
            pc.PimPamPumBeginCardDrag();
        }

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return base.CardEffect(pc, player, drop, cardIndex);
            yield return pc.ShotPimPamPum(player);
        }

        public override IEnumerator CardUsed(PlayerController pc)
        {
            yield return GameController.Instance.UsedCard<PimPamPum>(pc);
        }

        public override string ToString()
        {
            return "PimPamPum";
        }
    }
}