namespace Bang
{

    public class EndTurnButton : BangButton
    {

        protected override void Click()
        {
            PlayerController.LocalPlayer.EndTurnButton();
        }

    }
}