using UnityEngine;
using UnityEngine.UI;

namespace PimPamPum
{
    public class DeckView : SelectView, IDeckView
    {
        [SerializeField] Text deck = null;

        public void SetDeckSize(int cards)
        {
            deck.text = cards.ToString();
        }

        public override void Click()
        {
            PlayerController.LocalPlayer.PhaseOneOption(PhaseOneOption.Deck);
        }
    }
}