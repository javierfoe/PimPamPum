
namespace Bang
{
    public class SlabTheKiller : PlayerController
    {

        private const int missesToDodge = 2;

        public override void OnStartServer()
        {
            base.OnStartServer();
            MissesToDodge = missesToDodge;
        }

        protected override string Character()
        {
            return "SlabTheKiller";
        }

    }
}