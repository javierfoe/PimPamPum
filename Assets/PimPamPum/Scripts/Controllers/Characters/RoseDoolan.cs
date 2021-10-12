
namespace PimPamPum
{
    public class RoseDoolan : PlayerController
    {

        private const int scopeModifier = 1;

        protected override void Awake()
        {
            base.Awake();
            Scope = scopeModifier;
        }

    }
}
