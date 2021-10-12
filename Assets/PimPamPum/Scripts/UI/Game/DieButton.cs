namespace PimPamPum
{

    public class DieButton : Button
    {

        protected override void Click()
        {
            PlayerController.CurrentPlayableCharacter.WillinglyDie();
        }

    }
}
