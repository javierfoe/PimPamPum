
namespace Bang
{
    public abstract class PlayerCardConverter : PlayerController
    {
        public override void DisableCards()
        {
            OriginalHand();
            base.DisableCards();
        }
    }
}