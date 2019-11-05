namespace PimPamPum
{
    public class ConfirmButton : Button
    {
        protected override void Click()
        {
            PlayerController.LocalPlayer.MakeDecisionClient(Decision.Confirm);
        }
    }
}
