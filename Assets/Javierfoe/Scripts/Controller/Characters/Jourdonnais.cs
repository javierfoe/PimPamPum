
namespace Bang
{
    public class Jourdonnais : PlayerController
    {

        private const int barrels = 2;

        public override void OnStartServer()
        {
            base.OnStartServer();
            Barrels = barrels;
        }

        protected override string Character()
        {
            return "Jourdonnais";
        }

    }
}
