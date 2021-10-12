
namespace PimPamPum
{
    public class Jourdonnais : PlayerController
    {

        private const int barrels = 1;

        protected override void Awake()
        {
            base.Awake();
            Barrels = barrels;
        }

    }
}
