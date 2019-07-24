using System.Collections;
using System.Collections.Generic;

namespace PimPamPum
{

    public class GeneralStoreCoroutine : IEnumerator
    {

        private PlayerController[] players;
        private int nextPlayer, playersAlive;
        private float maxTime;

        public int NextPlayer
        {
            get { return nextPlayer; }
            private set
            {
                nextPlayer = value;
                Current = new GeneralStoreTimer(players[NextPlayer].connectionToClient, CardChoices, maxTime);
            }
        }

        public object Current { get; private set; }
        public List<Card> CardChoices { get; set; }
        public Card LastCard { get { return CardChoices[0]; } }

        public bool MoveNext()
        {
            NextPlayer = GameController.Instance.NextPlayerAlive(NextPlayer);
            bool res = playersAlive-- > 2;
            if (!res)
            {
                GameController.Instance.DisableSelectableCards();
            }
            return res;
        }

        public GeneralStoreCoroutine(PlayerController[] players, int start, List<Card> cards, float maxTime)
        {
            this.maxTime = maxTime;
            this.players = players;
            playersAlive = GameController.Instance.PlayersAlive;
            GameController.Instance.SetSelectableCards(cards);
            CardChoices = cards;
            NextPlayer = start;
        }

        public void Reset() { }

    }
}