using System.Collections;

namespace PimPamPum
{
    public class Missed : Card
    {
        public override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return null;
        }

        public override IEnumerator CardUsed(PlayerController pc)
        {
            yield return GameController.Instance.UsedCard<Missed>(pc);
        }

        public override string ToString()
        {
            return "Missed";
        }
    }
}