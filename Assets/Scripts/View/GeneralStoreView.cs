using UnityEngine.UI;

namespace Bang
{
    public class GeneralStoreView : CardView, IGeneralStoreCardView
    {
        private Button button;

        public void Enable(bool value)
        {
            if (!button)
            {
                button = GetComponent<Button>();
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