using System.Collections;

namespace PimPamPum
{
    public class Duel : Card
    {
        public override void BeginCardDrag(PlayerController pc)
        {
            base.BeginCardDrag(pc);
            pc.TargetOthers();
        }

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return base.CardEffect(pc, player, drop, cardIndex);
            yield return pc.Duel(player);
        }

        public override string ToString()
        {
            return "Duel";
        }
    }
}