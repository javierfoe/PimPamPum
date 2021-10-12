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

        public override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return base.CardEffect(pc, player, drop, cardIndex);
            yield return Shoot(pc, player);
        }

        protected virtual IEnumerator Shoot(PlayerController pc, int target)
        {
            yield return pc.ShootPimPamPum(target);
            pc.PimPamPumUsed();
        }

        public override IEnumerator CardUsed(PlayerController pc)
        {
            yield return GameController.UsedCard<PimPamPum>(pc);
        }

        public override string ToString()
        {
            return "PimPamPum";
        }
    }
}