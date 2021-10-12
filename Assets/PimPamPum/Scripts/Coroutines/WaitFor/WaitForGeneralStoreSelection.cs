﻿using System.Collections.Generic;

namespace PimPamPum
{
    public class WaitForGeneralStoreSelection : WaitFor
    {

        private int cardAmount;

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
            Finished(res);
            return res;
        }

        protected WaitForGeneralStoreSelection(PlayerController player, int cards) : base(player)
        {
            Choice = -1;
            cardAmount = cards;
        }

        public WaitForGeneralStoreSelection(PlayerController player, List<Card> cards) : this(player, cards.Count)
        {
            Cards = cards;
            GameController.EnableGeneralStoreCards(true);
        }

        public override void MakeDecisionCardIndex(int card)
        {
            Choice = card;
            NotChosenCards = new List<Card>(Cards);
            ChosenCard = Cards[card];
            NotChosenCards.RemoveAt(card);
        }

        protected override void Finished()
        {
            GameController.EnableGeneralStoreCards(false);
        }
    }
}