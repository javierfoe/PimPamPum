using UnityEngine;

namespace PimPamPum
{
    public class DiscardView : ClickView, IDiscardView
    {

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

        public void SetDiscardTop(CardValues cs)
        {
            discardTopCard.SetCard(cs);
        }

        public override void Click()
        {
            PlayerController.CurrentPlayableCharacter.PhaseOneDecision(Decision.Discard);
        }
    }
}