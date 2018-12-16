using UnityEngine.UI;

namespace Bang
{
    public class GeneralStoreCardView : CardView, IGeneralStoreCardView
    {
        private Button button;

        public void Enable(bool value)
        {
            if (!button)
            {
                button = GetComponent<Button>();
                button.onClick.AddListener(ClickGeneralStoreCard);
            }
            button.interactable = value;
        }

        private void ClickGeneralStoreCard()
        {
            PlayerController.LocalPlayer.ChooseGeneralStoreCard(index);
        }

    }
}