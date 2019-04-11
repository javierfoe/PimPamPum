namespace PimPamPum
{

    public class PassButton : Button
    {

        protected override void Click()
        {
            PlayerController.LocalPlayer.PassButton();
        }

    }
}