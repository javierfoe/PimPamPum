using System.Collections;

namespace PimPamPum
{
    public class Panic : CatBalou
    {
        protected override void BeginStealCardDrag(PlayerController pc)
        {
            pc.PanicBeginCardDrag();
        }

        protected override IEnumerator StealCard(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return pc.Panic(player, drop, cardIndex);
        }

        public override string ToString()
        {
            return "Panic";
        }
    }
}