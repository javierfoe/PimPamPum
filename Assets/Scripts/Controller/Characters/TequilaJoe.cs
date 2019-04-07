
namespace PimPamPum
{
    public class TequilaJoe : PlayerController
    {

        private const int beerHeal = 2;

        public override void OnStartServer()
        {
            base.OnStartServer();
            BeerHeal = beerHeal;
        }


        protected override string Character()
        {
            return "Tequila Joe";
        }

    }
}