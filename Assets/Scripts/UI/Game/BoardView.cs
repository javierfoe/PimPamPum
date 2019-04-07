using UnityEngine;
using UnityEngine.UI;

namespace PimPamPum
{

    public class BoardView : DropView, IBoardView
    {

        [SerializeField] private CardView discardStackTop = null;
        [SerializeField] private Text deck = null;
        [SerializeField] private GeneralStoreListView generalStore = null;

        private CardStruct defaultCard = new CardStruct { name = "", color = Color.black, suit = Suit.Null, rank = Rank.Null };

        private ICardView discardTopCard;

        protected override void Awake()
        {
            base.Awake();
            drop = Drop.Trash;
            discardTopCard = discardStackTop;
        }

        public override void SetTargetable(bool value)
        {
            Droppable = value;
            base.SetTargetable(value);
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
            discardTopCard.SetCard(cs);
        }

        public void EmptyDiscardStack()
        {
            discardTopCard.SetCard(defaultCard);
        }
    }
}