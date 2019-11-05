using System.Collections;

namespace PimPamPum
{
    public class CatBalou : Card
    {
        public override void BeginCardDrag(PlayerController pc)
        {
            base.BeginCardDrag(pc);
            BeginStealCardDrag(pc);
        }

        protected virtual void BeginStealCardDrag(PlayerController pc)
        {
            pc.CatBalouBeginCardDrag();
        }

        public override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return StealCard(pc, player, drop, cardIndex);
        }

        protected virtual IEnumerator StealCard(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return pc.CatBalou(player, drop, cardIndex);
        }

        public override IEnumerator CardUsed(PlayerController pc)
        {
            yield return GameController.Instance.UsedCard<CatBalou>(pc);
        }

        public override string ToString()
        {
            return "Cat Balou";
        }
    }
}