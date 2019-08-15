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

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return StealCard(pc, player, drop, cardIndex);
        }

        protected virtual IEnumerator StealCard(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return pc.CatBalou(player, drop, cardIndex);
        }

        public override string ToString()
        {
            return "Cat Balou";
        }
    }
}