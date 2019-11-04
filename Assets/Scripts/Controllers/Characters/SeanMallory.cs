
namespace PimPamPum
{
    public class SeanMallory : PlayerController
    {

        private const int cardLimit = 10;

        protected override int CardLimit()
        {
            return cardLimit;
        }

    }
}