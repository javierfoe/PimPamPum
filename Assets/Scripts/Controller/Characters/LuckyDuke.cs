
namespace PimPamPum
{

    public class LuckyDuke : PlayerController
    {

        public override void OnStartServer()
        {
            base.OnStartServer();
            DrawEffectCards = 2;
        }

    }

}