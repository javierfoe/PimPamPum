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

        protected WaitForGeneralStoreSelection(NetworkConnection conn, int cards) : base()
        {
            this.conn = conn;
            Choice = -1;
            cardAmount = cards;
        }

        public WaitForGeneralStoreSelection(NetworkConnection conn, List<Card> cards) : this(conn, cards.Count)
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