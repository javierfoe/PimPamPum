using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bang
{

    public class BoardView : MonoBehaviour, IBoardView
    {

        [SerializeField] private CardView discardStackTop = null;
        [SerializeField] private Text deck = null;
        [SerializeField] private GeneralStoreCardListView generalStore = null;

        private ICardView discardTopCard;

        void Start()
        {
            discardTopCard = discardStackTop;
            EnableGeneralStore(false);
        }

        public void EnableGeneralStore(bool value)
        {
            generalStore.gameObject.SetActive(value);
        }

        public void EnableGeneralStoreCards(bool value)
        {
            generalStore.EnableCards(value);
        }

        public void AddGeneralStoreCard(int index, string name, Suit suit, Rank rank, Color color)
        {
            generalStore.AddCardView(index, name, suit, rank, color);
        }

        public void RemoveGeneralStoreCard(int index)
        {
            generalStore.RemoveCardView(index);
        }

        public void SetDeckSize(int cards)
        {
            deck.text = cards.ToString();
        }

        public void SetDiscardTop(string name, Suit suit, Rank rank, Color color)
        {
            discardTopCard.SetName(name, color);
            discardTopCard.SetRank(rank);
            discardTopCard.SetSuit(suit);
        }

        public void EmptyDiscardStack()
        {
            discardTopCard.SetName("", Color.black);
            discardTopCard.SetRank(Rank.Null);
            discardTopCard.SetSuit(Suit.Null);
        }
    }
}