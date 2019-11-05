using UnityEngine;
using UnityEngine.UI;

namespace PimPamPum
{
    public class DeckView : ClickView, IDeckView
    {
        [SerializeField] Image image = null;
        [SerializeField] Text deck = null;

        public void SetDeckSize(int cards)
        {
            deck.text = cards.ToString();
        }

        public override void EnableClick(bool value)
        {
            base.EnableClick(value);
            image.color = value ? Color.magenta : new Color(0, 0, 0, 0);
        }

        public override void Click()
        {
            PlayerController.LocalPlayer.PhaseOneDecision(Decision.Deck);
        }
    }
}