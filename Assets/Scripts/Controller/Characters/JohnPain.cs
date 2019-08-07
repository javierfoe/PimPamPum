
namespace PimPamPum
{
    public class JohnPain : PlayerController
    {

        public override bool DrawEffectPickup()
        {
            return !IsDead && hand.Count < 6;
        }

    }
}