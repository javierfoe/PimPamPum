using System.Collections;

namespace PimPamPum
{
    public class Missed : Card
    {
        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return null;
        }

        public override string ToString()
        {
            return "Missed";
        }
    }
}