namespace PimPamPum
{

    public class BarrelButton : Button
    {

        protected override void Click()
        {
            PlayerController.LocalPlayer.UseBarrel();
        }

    }
}
