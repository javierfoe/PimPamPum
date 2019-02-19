using System.Collections;

namespace Bang
{
    public class TerenKill : PlayerController
    {

        protected override IEnumerator DieTrigger(int killer)
        {
            yield return GameController.Instance.DrawEffect(PlayerNumber);
            if (GameController.Instance.DrawnCard.Suit != Suit.Spades)
            {
                yield return BangEvent(this + " draws! to stay alive: " + GameController.Instance.DrawnCard);
                HP = 1;
                Draw(1);
            }
            else
            {
                yield return BangEvent(this + " draws! and dies: " + GameController.Instance.DrawnCard);
                yield return base.DieTrigger(killer);
            }
        }

        protected override string Character()
        {
            return "Teren Kill";
        }

    }
}