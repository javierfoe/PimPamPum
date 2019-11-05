
namespace PimPamPum
{
    public class RoseDoolan : PlayerController
    {

        private const int scopeModifier = 1;

        public override void OnStartServer()
        {
            base.OnStartServer();
            Scope = scopeModifier;
        }

    }
}
