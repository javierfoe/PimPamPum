
namespace PimPamPum
{
    public class Jourdonnais : PlayerController
    {

        private const int barrels = 1;

        public override void OnStartServer()
        {
            base.OnStartServer();
            Barrels = barrels;
        }

    }
}
