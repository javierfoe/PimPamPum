using Mirror;
using System.Collections.Generic;

namespace PimPamPum
{
    public class WaitForCardSelection : WaitForGeneralStoreSelection
    {
        public WaitForCardSelection(NetworkConnection conn, int cards) :
            this(conn, GameController.Instance.DrawCards(cards))
        { }

        public WaitForCardSelection(NetworkConnection conn, List<Card> cards) : base(conn, cards.Count)
        {
            GameController.Instance.SetSelectableCards(cards, conn);
            Cards = cards;
        }

        protected override void FinishedCoroutine()
        {
            GameController.Instance.RemoveSelectableCardsAndDisable(conn);
        }
    }
}