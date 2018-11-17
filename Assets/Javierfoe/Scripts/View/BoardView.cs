using UnityEngine;
using UnityEngine.UI;

namespace Bang
{

    public class BoardView : MonoBehaviour, IBoardView
    {

        [SerializeField] private CardView discardStackTop = null;
        [SerializeField] private Text deck = null;
        private ICardView discardTopCard;

        void Start()
        {
            discardTopCard = discardStackTop;
        }

        public void SetDeckSize(int cards)
        {
            deck.text = cards.ToString();
        }

        public void SetDiscardTop(string name, ESuit suit, ERank rank, Color color)
        {
            discardTopCard.SetName(name, color);
            discardTopCard.SetRank(rank);
            discardTopCard.SetSuit(suit);
        }

        public void EmptyDiscardStack()
        {
            discardTopCard.SetName("", Color.black);
            discardTopCard.SetRank(ERank.NULL);
            discardTopCard.SetSuit(ESuit.NULL);
        }
    }
}