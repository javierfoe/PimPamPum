namespace PimPamPum
{
    public class ConfirmButton : Button
    {
        protected override void Click()
        {
            PlayerController.CurrentPlayableCharacter.MakeDecision(Decision.Confirm);
        }
    }
}
