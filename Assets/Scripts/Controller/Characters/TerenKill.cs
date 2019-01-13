using System.Collections;

namespace Bang
{
    public class TerenKill : PlayerController
    {

        public override IEnumerator Die(int killer)
        {
            yield return GameController.DrawEffect(PlayerNumber);
            if (GameController.DrawnCard.Suit != Suit.Spades)
            {
                yield return BangEvent(this + " draws! to stay alive: " + GameController.DrawnCard);
                HP = 1;
                Draw(1);
            }
            else
            {
                yield return BangEvent(this + " draws! and dies: " + GameController.DrawnCard);
                yield return base.Die(killer);
            }
        }

        protected override string Character()
        {
            return "Teren Kill";
        }

    }
}