
namespace PimPamPum
{
    public class MollyStark : PlayerController
    {

        protected override void CardUsedOutOfTurn()
        {
            Draw();
        }

    }
}