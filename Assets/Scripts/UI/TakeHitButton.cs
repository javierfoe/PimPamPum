namespace Bang
{

    public class TakeHitButton : BangButton
    {

        protected override void Click()
        {
            PlayerController.LocalPlayer.TakeHit();
        }

    }
}
