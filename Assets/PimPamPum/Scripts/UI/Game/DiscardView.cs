using UnityEngine;

namespace PimPamPum
{
    public class DiscardView : ClickView, IDiscardView
    {
        private static readonly CardStruct defaultCard = new CardStruct { name = "", color = Color.black, suit = Suit.Null, rank = Rank.Null };

        [SerializeField] private GameObject discardTopCardGO = null;

        private ICardView discardTopCard;

        void Awake()
        {
            discardTopCard = discardTopCardGO.GetComponent<ICardView>();
        }

        public override void EnableClick(bool value)
        {
            base.EnableClick(value);
            discardTopCard.EnableClick(value);
        }

        public void SetDiscardTop(CardStruct cs)
        {
            discardTopCard.SetCard(cs);
        }

        public void EmptyDiscardStack()
        {
            discardTopCard.SetCard(defaultCard);
        }

        public override void Click()
        {
            PlayerController.LocalPlayer.PhaseOneDecision(Decision.Discard);
        }
    }
}