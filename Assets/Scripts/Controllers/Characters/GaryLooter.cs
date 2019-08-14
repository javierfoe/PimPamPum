
namespace PimPamPum
{
    public class GaryLooter : PlayerController
    {

        public override bool EndTurnDiscardPickup(int player)
        {
            return !IsDead && PlayerNumber != player;
        }

    }
}