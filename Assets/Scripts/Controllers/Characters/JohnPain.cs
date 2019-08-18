
namespace PimPamPum
{
    public class JohnPain : PlayerController
    {

        public override bool DrawEffectPickup(int player)
        {
            return !IsDead && hand.Count < 6;
        }

    }
}