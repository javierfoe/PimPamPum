using System.Collections.Generic;

namespace PimPamPum
{
    public class WaitForCardSelection : WaitForGeneralStoreSelection
    {
        public WaitForCardSelection(PlayerController player, int cards) :
            this(player, GameController.DrawCards(cards))
        { }

        public WaitForCardSelection(PlayerController player, List<Card> cards) : base(player, cards.Count)
        {
            GameController.SetSelectableCards(cards);
            Cards = cards;
        }

        protected override void Finished()
        {
            GameController.RemoveSelectableCardsAndDisable();
        }
    }
}