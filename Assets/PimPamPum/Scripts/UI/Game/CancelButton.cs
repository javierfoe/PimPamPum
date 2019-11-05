namespace PimPamPum
{
    public class CancelButton : Button
    {
        protected override void Click()
        {
            PlayerController.LocalPlayer.MakeDecisionClient(Decision.Cancel);
        }
    }
}
