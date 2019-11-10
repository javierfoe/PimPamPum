using Mirror;
using System.Collections.Generic;

namespace PimPamPum
{
    public class WaitForGeneralStoreSelection : WaitFor
    {

        private int cardAmount;
        protected NetworkConnection conn;

        public int Choice { get; private set; }
        public Card ChosenCard { get; private set; }
        public List<Card> NotChosenCards { get; private set; }
        public List<Card> Cards { get; protected set; }

        public override bool MoveNext()
        {
            bool res = base.MoveNext() && Choice < 0;
            if (!res && Choice < 0)
            {
                int random = UnityEngine.Random.Range(0, cardAmount);
                MakeDecisionCardIndex(random);
            }
            if (!res)
            {
                FinishedCoroutine();
            }
            return res;
        }

        protected WaitForGeneralStoreSelection(PlayerController player, int cards) : base(player)
        {
            conn = player.connectionToClient;
            Choice = -1;
            cardAmount = cards;
        }

        public WaitForGeneralStoreSelection(PlayerController player, List<Card> cards) : this(player, cards.Count)
        {
            Cards = cards;
            GameController.Instance.EnableGeneralStoreCards(conn, true);
        }

        public override void MakeDecisionCardIndex(int card)
        {
            Choice = card;
            NotChosenCards = new List<Card>(Cards);
            ChosenCard = Cards[card];
            NotChosenCards.RemoveAt(card);
        }

        protected virtual void FinishedCoroutine()
        {
            GameController.Instance.EnableGeneralStoreCards(conn, false);
        }
    }
}