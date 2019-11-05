using System.Collections;

namespace PimPamPum
{
    public class SidKetchum : PlayerDiscardTwoCards
    {
        protected override bool ValidState()
        {
            return base.ValidState() || State == State.Dying;
        }

        protected override void EnableConfirmOptions(Card one, Card two)
        {
            EnableConfirmButton(true);
        }

        protected override IEnumerator SpecialAction(int target)
        {
            Heal();
            yield return null;
        }
    }
}