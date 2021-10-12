namespace PimPamPum
{
    public class CancelButton : Button
    {
        protected override void Click()
        {
            PlayerController.CurrentPlayableCharacter.MakeDecision(Decision.Cancel);
        }
    }
}
