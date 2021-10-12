
namespace PimPamPum
{
    public class TequilaJoe : PlayerController
    {

        private const int beerHeal = 2;

        protected override void Awake()
        {
            base.Awake();
            BeerHeal = beerHeal;
        }

    }
}