using UnityEngine;

namespace PimPamPum
{
    public class CreateLobby : Button
    {
        private int players;

        protected override void Start()
        {
            base.Start();
            PlayerAmountSlider.AddListener(SetPlayers);
        }

        protected override void Click()
        {
            LobbyController.Instance.CreateLobby(players);
        }

        private void SetPlayers(float value)
        {
            players = (int)value;
            Debug.Log("Set Players Steam: " + value);
        }
    }
}