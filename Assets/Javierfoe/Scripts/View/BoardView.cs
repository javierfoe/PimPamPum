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

        public void AddGeneralStoreCard(int index, CardStruct cs)
        {
            generalStore.AddCardView(index, cs);
        }

        public void RemoveGeneralStoreCard(int index)
        {
            generalStore.RemoveCardView(index);
        }

        public void SetDeckSize(int cards)
        {
            deck.text = cards.ToString();
        }

        public void SetDiscardTop(CardStruct cs)
        {
            discardTopCard.SetName(cs.name, cs.color);
            discardTopCard.SetRank(cs.rank);
            discardTopCard.SetSuit(cs.suit);
        }

        public void EmptyDiscardStack()
        {
            discardTopCard.SetName("", Color.black);
            discardTopCard.SetRank(Rank.Null);
            discardTopCard.SetSuit(Suit.Null);
        }
    }
}