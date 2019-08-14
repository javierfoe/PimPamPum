using System.Collections.Generic;

namespace PimPamPum
{

    public class GeneralStoreCoroutine : FirstTimeEnumerator
    {

        private PlayerController[] players;
        private int nextPlayer, playersAlive;
        private bool lastCard;

        public int NextPlayer
        {
            get { return nextPlayer; }
            private set
            {
                nextPlayer = value;
                Current = new WaitForGeneralStoreSelection(players[NextPlayer].connectionToClient, CardChoices);
            }
        }

        public List<Card> CardChoices { get; set; }
        public Card LastCard { get { return CardChoices[0]; } }

        public override bool MoveNext()
        {
            if (FirstTime) return true;
            WaitForGeneralStoreSelection generalStoreTimer = Current as WaitForGeneralStoreSelection;
            if (generalStoreTimer != null)
            {
                CardChoices = generalStoreTimer.NotChosenCards;
                Current = GameController.Instance.GetCardGeneralStore(NextPlayer, generalStoreTimer.Choice, generalStoreTimer.ChosenCard);
                return true;
            }
            bool res = playersAlive-- > 2;
            if (res)
            {
                NextPlayer = GameController.Instance.NextPlayerAlive(NextPlayer);
                return true;
            }
            if (!res && !lastCard)
            {
                lastCard = true;
                nextPlayer = GameController.Instance.NextPlayerAlive(NextPlayer);
                Current = GameController.Instance.GetCardGeneralStore(NextPlayer, 0, LastCard);
                GameController.Instance.DisableSelectableCards();
                return true;
            }
            return res;
        }

        public GeneralStoreCoroutine(PlayerController[] players, int start, List<Card> cards)
        {
            this.players = players;
            lastCard = false;
            playersAlive = GameController.Instance.PlayersAlive;
            GameController.Instance.SetSelectableCards(cards);
            CardChoices = cards;
            NextPlayer = start;
        }

    }
}