using System.Collections.Generic;

namespace PimPamPum
{
    public class GeneralStoreCoroutine : Enumerator
    {
        private PlayerController[] players;
        private int nextPlayer, pendingPlayers;
        private bool lastCard;

        public List<Card> CardChoices { get; set; }
        public Card LastCard { get { return CardChoices[0]; } }

        public override bool MoveNext()
        {
            WaitForGeneralStoreSelection generalStoreTimer = Current as WaitForGeneralStoreSelection;
            if (generalStoreTimer != null)
            {
                CardChoices = generalStoreTimer.NotChosenCards;
                Current = GameController.Instance.GetCardGeneralStore(nextPlayer, generalStoreTimer.Choice, generalStoreTimer.ChosenCard);
                nextPlayer = GameController.Instance.NextPlayerAlive(nextPlayer);
                pendingPlayers--;
                return true;
            }
            bool res = pendingPlayers > 1;
            if (res)
            {
                Current = new WaitForGeneralStoreSelection(players[nextPlayer], CardChoices);
                return true;
            }
            if (!res && !lastCard)
            {
                lastCard = true;
                Current = GameController.Instance.GetCardGeneralStore(nextPlayer, 0, LastCard);
                GameController.Instance.DisableSelectableCards();
                return true;
            }
            return res;
        }

        public GeneralStoreCoroutine(PlayerController[] players, int start, List<Card> cards)
        {
            this.players = players;
            lastCard = false;
            pendingPlayers = cards.Count;
            CardChoices = cards;
            nextPlayer = start;
        }
    }
}