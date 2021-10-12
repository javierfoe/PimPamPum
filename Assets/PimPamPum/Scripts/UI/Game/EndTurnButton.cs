
namespace PimPamPum
{

    public class EndTurnButton : Button
    {

        protected override void Click()
        {
            PlayerController.CurrentPlayableCharacter.EndTurnButton();
        }

    }
}