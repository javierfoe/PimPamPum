
namespace PimPamPum
{
    public class PaulRegret : PlayerController
    {

        private const int rangeModifier = 1;

        public override void OnStartServer()
        {
            base.OnStartServer();
            RangeModifier = rangeModifier;
        }

    }
}
