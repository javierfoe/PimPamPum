
namespace Bang
{
    public class PaulRegret : PlayerController
    {

        private const int rangeModifier = 1;

        public override void OnStartServer()
        {
            base.OnStartServer();
            RangeModifier = rangeModifier;
        }

        protected override string Character()
        {
            return "Paul Regret";
        }

    }
}
