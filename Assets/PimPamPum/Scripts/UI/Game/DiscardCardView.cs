

namespace PimPamPum
{
    public class DiscardCardView : CardView
    {
        public override void Click()
        {
            PlayerController.CurrentPlayableCharacter.PhaseOneDecision(Decision.Discard);
        }
    }
}