using System.Collections;

namespace PimPamPum
{
    public class TerenKill : PlayerController
    {

        protected override IEnumerator DieTrigger(int killer)
        {
            yield return GameController.Instance.DrawEffect(PlayerNumber);
            if (GameController.Instance.DrawnCard.Suit != Suit.Spades)
            {
                yield return PimPamPumEvent(this + " draws! to stay alive: " + GameController.Instance.DrawnCard);
                HP = 1;
                Draw(1);
            }
            else
            {
                yield return PimPamPumEvent(this + " draws! and dies: " + GameController.Instance.DrawnCard);
                yield return base.DieTrigger(killer);
            }
        }

        protected override string Character()
        {
            return "Teren Kill";
        }

    }
}