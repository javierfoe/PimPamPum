
namespace Bang
{
    public class MollyStark : PlayerController
    {

        protected override void CardUsedOutOfTurn()
        {
            Draw(1);
        }

        protected override string Character()
        {
            return "Molly Stark";
        }

    }
}