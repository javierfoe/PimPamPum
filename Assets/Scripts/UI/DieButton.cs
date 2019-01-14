namespace Bang
{

    public class DieButton : Button
    {

        protected override void Click()
        {
            PlayerController.LocalPlayer.WillinglyDie();
        }

    }
}
