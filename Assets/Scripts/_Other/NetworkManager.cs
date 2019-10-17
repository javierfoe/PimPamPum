using Mirror;
using System.Collections.Generic;

namespace PimPamPum
{
    public class NetworkManager : Mirror.NetworkManager
    {

        private List<int> availableCharacters;
        private int currentPlayers, maxPlayers;
        private PlayerController[] players;

        public int AvailableCharacter
        {
            get
            {
                int index = UnityEngine.Random.Range(0, availableCharacters.Count);
                int res = availableCharacters[index];
                availableCharacters.RemoveAt(index);
                return res;
            }
            set
            {
                availableCharacters = new List<int>();
                for (int i = 0; i < value; i++)
                {
                    availableCharacters.Add(i);
                }
            }
        }

        public void SetPlayerAmount(float amount)
        {
            maxPlayers = (int)amount;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            currentPlayers = 0;
            AvailableCharacter = spawnPrefabs.Count;
            PlayerAmountSlider.AddListener(SetPlayerAmount);
            players = new PlayerController[maxPlayers];
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            if (currentPlayers == maxPlayers)
            {
                conn.Disconnect();
                return;
            }

            PlayerController playerController = Instantiate(spawnPrefabs[AvailableCharacter]).GetComponent<PlayerController>();
            playerController.PlayerNumber = currentPlayers;
            NetworkServer.AddPlayerForConnection(conn, playerController.gameObject);
            players[currentPlayers++] = playerController;

            playerController.TargetSetLocalPlayer(conn, maxPlayers);

            if (currentPlayers == maxPlayers)
            {
                GameController gc = FindObjectOfType<GameController>();
                gc.SetMatch(players);
            }
        }
    }
}