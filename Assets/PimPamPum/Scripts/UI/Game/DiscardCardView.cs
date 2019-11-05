

namespace PimPamPum
{
    public class DiscardCardView : CardView
    {
        public override void Click()
        {
            PlayerController.LocalPlayer.PhaseOneDecision(Decision.Discard);
        }
    }
}