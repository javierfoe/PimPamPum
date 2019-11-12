using Mirror;
using System.Collections.Generic;

namespace PimPamPum
{
    public class WaitForCardSelection : WaitForGeneralStoreSelection
    {
        public WaitForCardSelection(PlayerController player, int cards) :
            this(player, GameController.Instance.DrawCards(cards))
        { }

        public WaitForCardSelection(PlayerController player, List<Card> cards) : base(player, cards.Count)
        {
            GameController.Instance.SetSelectableCards(cards, conn);
            Cards = cards;
        }

        protected override void Finished()
        {
            GameController.Instance.RemoveSelectableCardsAndDisable(conn);
        }
    }
}