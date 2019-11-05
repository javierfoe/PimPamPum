
namespace PimPamPum
{
    public class SlabTheKiller : PlayerController
    {

        private const int missesToDodge = 2;

        public override void OnStartServer()
        {
            base.OnStartServer();
            MissesToDodge = missesToDodge;
        }

    }
}