
namespace PimPamPum
{
    public class SlabTheKiller : PlayerController
    {

        private const int missesToDodge = 2;

        protected override void Awake()
        {
            base.Awake();
            MissesToDodge = missesToDodge;
        }

    }
}