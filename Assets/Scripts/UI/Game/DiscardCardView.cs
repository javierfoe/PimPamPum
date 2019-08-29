

namespace PimPamPum
{
    public class DiscardCardView : CardView
    {
        public override void Click()
        {
            PlayerController.LocalPlayer.PhaseOneOptionDecision(PhaseOneOption.Discard);
        }
    }
}