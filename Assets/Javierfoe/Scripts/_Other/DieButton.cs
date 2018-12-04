namespace Bang
{

    public class DieButton : BangButton
    {

        protected override void Click()
        {
            PlayerController.LocalPlayer.WillinglyDie();
        }

    }
}
