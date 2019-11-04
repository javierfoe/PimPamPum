
namespace PimPamPum
{
    public class PixiePete : PlayerController
    {

        public override void OnStartServer()
        {
            base.OnStartServer();
            Phase1CardsDrawn = 3;
        }

    }
}
