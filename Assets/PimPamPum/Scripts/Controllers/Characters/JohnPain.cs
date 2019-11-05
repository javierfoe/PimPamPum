
namespace PimPamPum
{
    public class JohnPain : PlayerController
    {

        public override bool DrawEffectPickup(int player)
        {
            return !IsDead && Hand.Count < 6;
        }

    }
}