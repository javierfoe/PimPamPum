
namespace PimPamPum
{
    public class PaulRegret : PlayerController
    {

        private const int rangeModifier = 1;

        protected override void Awake()
        {
            base.Awake();
            RangeModifier = rangeModifier;
        }

    }
}
