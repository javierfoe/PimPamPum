
namespace Bang
{
    public class MollyStark : PlayerController
    {

        protected override void CardUsedOutOfTurn()
        {
            Draw();
        }

        protected override string Character()
        {
            return "Molly Stark";
        }

    }
}