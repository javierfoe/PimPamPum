
namespace Bang
{
    public class PixiePete : PlayerController
    {

        private const int drawCards = 3;

        public override void OnStartServer()
        {
            base.OnStartServer();
            Phase1CardsDrawn = drawCards;
        }

        protected override string Character()
        {
            return "Pixie Pete";
        }

    }
}
