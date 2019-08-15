using Mirror;

namespace PimPamPum
{
    public class WaitForCardSelection : WaitForGeneralStoreSelection
    {
        public WaitForCardSelection(NetworkConnection conn, int cards) : base(conn, cards)
        {
            Cards = GameController.Instance.DrawChooseCards(cards, conn);
        }

        protected override void FinishedCoroutine()
        {
            GameController.Instance.RemoveSelectableCardsAndDisable(conn);
        }
    }
}