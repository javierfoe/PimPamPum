
namespace PimPamPum
{

    public class SkipButton : Button
    {

        protected override void Click()
        {
            PlayerController.CurrentPlayableCharacter.PassButton();
        }

    }
}