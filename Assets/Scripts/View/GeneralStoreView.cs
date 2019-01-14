using UnityEngine.UI;

namespace Bang
{
    public class GeneralStoreView : CardView, IGeneralStoreCardView
    {
        private UnityEngine.UI.Button button;

        public void Enable(bool value)
        {
            if (!button)
            {
                button = GetComponent<UnityEngine.UI.Button>();
                button.onClick.AddListener(ClickGeneralStoreCard);
            }
            Playable(value);
            button.interactable = value;
        }

        public override void Playable(bool value)
        {
            base.Playable(value);
            Draggable = false;
        }

        private void ClickGeneralStoreCard()
        {
            PlayerController.LocalPlayer.ChooseGeneralStoreCard(index);
        }

    }
}