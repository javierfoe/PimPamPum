
using System.Collections;

namespace Bang
{
    public class ColoradoBill : PlayerController
    {

        public override IEnumerator ShotBang(int target)
        {
            bangsUsed++;
            yield return GameController.DrawEffect(PlayerNumber);
            if (GameController.DrawnCard.Suit == Suit.Spades)
            {
                yield return BangEvent(this + " has shot an undodgeable Bang! " + GameController.DrawnCard);
                PlayerController pcTarget = GameController.GetPlayerController(target);
                yield return pcTarget.Hit(PlayerNumber, MissesToDodge);
            }
            else
            {
                yield return BangEvent(this + " has shot an standard Bang! " + GameController.DrawnCard);
                yield return GameController.Bang(PlayerNumber, target, MissesToDodge);
            }
        }

        protected override string Character()
        {
            return "Colorado Bill";
        }

    }
}